using System.Text.Json;
using API.Data;
using API.Extensions;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.ServiceErrors;

namespace API.Features.Screenings.CreateScreening;

public class CreateScreeningRequestHandler(
    CinemaDbContext context,
    ILogger<CreateScreeningRequestHandler> logger,
    TimeProvider timeProvider)
    : IRequestHandler<CreateScreeningRequest, ErrorOr<CreateScreeningResponse>>
{
    public async Task<ErrorOr<CreateScreeningResponse>> Handle(CreateScreeningRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Trying to create screening for movie {movieId}.\nTheater:{theaterId}.\nScreeningDate:{screeningDate}.",
            request.MovieId, request.TheaterId, request.ScreeningDate.FormatScreeningTime());

        var validationResult = await new CreateScreeningRequestValidator(timeProvider)
            .ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrorList();
            logger.LogInformation("Validation failed for {0}. Errors: {1}", nameof(CreateScreeningRequest),
                errors);
            return errors;
        }

        var movie = await context.Movies.FindAsync(request.MovieId);
        if (movie is null)
        {
            logger.LogError("Movie not found. ID: {movieId}", request.MovieId);
            return Errors.Movie.NotFound;
        }

        var theater = await context.Theaters.FindAsync(request.TheaterId);
        if (theater is null)
        {
            logger.LogError("Theater not found. ID: {theaterId}", request.TheaterId);
            return Errors.Theater.NotFound;
        }

        // check for collisions.
        // there should be no other screenings in this theater within 30 minutes of its start and before it has ended.
        var isScreeningTimeValid = await
            IsScreenTimeColliding(request.ScreeningDate, theater.TheaterId, movie.MovieId, movie.DurationInMinutes);
        if (isScreeningTimeValid)
        {
            logger.LogError("There are time conflicts for this screening. Check the logs.");
            return Errors.Screening.TimeConflict;
        }

        var screening = ScreeningMapper.ToScreeningObject(request, movie.DurationInMinutes);

        context.Add(screening);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Created screening for Movie {movieId} at theater {theaterId} at date: {screeningDate}.\nID: {screeningId}",
            movie.MovieId,
            theater.TheaterId,
            request.ScreeningDate.FormatScreeningTime(),
            screening.ShowtimeId);
        // todo generate seats
        return new CreateScreeningResponse(screening.ShowtimeId);
    }

    private async Task<bool> IsScreenTimeColliding(DateTime requestScreeningTime,
        string theaterId,
        string movieId,
        int movieDurationInMinutes)
    {
        const int collisionWindowStart = -5;
        const int collisionWindowEnd = 5;

        var moviesInTheater = await context.Screenings.Where(x => x.TheatreId == theaterId).ToListAsync();
        logger.LogInformation("Screenings scheduled: {moviesInTheater}", JsonSerializer.Serialize(moviesInTheater));

        // todo: debug here and check code pls.
        logger.LogInformation(
            "Checking for screening collisions at theater {theaterId} for screening at {screeningDate}, for Movie: {movieId}.",
            theaterId,
            requestScreeningTime.FormatScreeningTime(),
            movieId);

        // Check for collisions at the start of a screening (within a 10-minute window around the requested screening time)
        var anyCollisionsAtStart = await context.Screenings
            .AnyAsync(x =>
                x.StartTime >= requestScreeningTime.AddMinutes(collisionWindowStart) &&
                x.StartTime < requestScreeningTime.AddMinutes(collisionWindowEnd) &&
                x.TheatreId == theaterId);
        logger.LogInformation("Is this screening scheduled to start when another starts? {anyCollisionsAtStart}",
            anyCollisionsAtStart);

        // Check for collisions during the requested screening (within the movie's duration plus 10 minutes)
        var anyCollisionsDuringScreening = await context.Screenings
            .AnyAsync(x =>
                x.StartTime > requestScreeningTime.AddMinutes(collisionWindowStart) &&
                x.StartTime <= requestScreeningTime.AddMinutes(movieDurationInMinutes).AddMinutes(collisionWindowEnd) &&
                x.TheatreId == theaterId);
        logger.LogInformation(
            "Are there screenings scheduled to start when this is still showing? {anyCollisionsDuringScreening}",
            anyCollisionsDuringScreening);

        // // Check for collisions at the end of a screening (within 10 minutes after the requested screening ends)
        // var anyCollisionsAtEnd = await context.Screenings
        //     .AnyAsync(x =>
        //         x.StartTime > requestScreeningTime.AddMinutes(movieDurationInMinutes).AddMinutes(collisionWindowStart) &&
        //         x.StartTime <= requestScreeningTime.AddMinutes(movieDurationInMinutes).AddMinutes(collisionWindowEnd) &&
        //         x.TheatreId == theaterId);
        // logger.LogInformation("Any collisions at end: {anyCollisionsAtEnd}", anyCollisionsAtEnd);

        var anyScreeningsAtProposedStartTime = await context.Screenings
            .AnyAsync(x =>
                x.StartTime <= requestScreeningTime &&
                x.EndTime >= requestScreeningTime &&
                x.TheatreId == theaterId
            );
        logger.LogInformation(
            "Are there screenings still showing at the time this is scheduled to start? {anyEndingWhenScreeningStarts}",
            anyScreeningsAtProposedStartTime);

        // check for movies ending when this screening starts
        var anyEndingWhenScreeningStarts = await context.Screenings
            .AnyAsync(x =>
                x.EndTime >= requestScreeningTime.AddMinutes(collisionWindowStart) &&
                x.EndTime < requestScreeningTime.AddMinutes(collisionWindowEnd) &&
                x.TheatreId == theaterId);
        logger.LogInformation("Is this screening scheduled to start when another ends? {anyEndingWhenScreeningStarts}",
            anyEndingWhenScreeningStarts);

        return anyCollisionsAtStart ||
               anyCollisionsDuringScreening ||
               anyScreeningsAtProposedStartTime ||
               anyEndingWhenScreeningStarts;
    }
}