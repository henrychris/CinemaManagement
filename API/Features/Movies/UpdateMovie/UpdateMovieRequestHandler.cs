using API.Data;
using API.Extensions;
using ErrorOr;
using FluentValidation;
using MediatR;
using Shared.ServiceErrors;

namespace API.Features.Movies.UpdateMovie;

public class UpdateMovieRequestHandler(
    CinemaDbContext context,
    IValidator<UpdateMovieRequest> validator,
    ILogger<UpdateMovieRequestHandler> logger) : IRequestHandler<UpdateMovieRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying to update a movie. Request: {0}", request);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrorList();
            logger.LogInformation("Validation failed for {0}. Errors: {1}", nameof(UpdateMovieRequest),
                errors);
            return errors;
        }

        var movie = await context.Movies.FindAsync(request.MovieId);
        if (movie is null)
        {
            logger.LogInformation("Movie not found. ID: {0}", request.MovieId);
            return Errors.Movie.NotFound;
        }

        movie.Title = request.Title ?? movie.Title;
        movie.Description = request.Description ?? movie.Description;
        movie.DurationInMinutes = request.DurationInMinutes ?? movie.DurationInMinutes;
        movie.ReleaseDate = request.ReleaseDate ?? movie.ReleaseDate;
        movie.Genres = request.Genres ?? movie.Genres;
        movie.Rating = request.Rating ?? movie.Rating;
        movie.Director = request.Director ?? movie.Director;

        context.Update(movie);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Movie {0} has been updated.", movie.MovieId);
        return Result.Updated;
    }
}