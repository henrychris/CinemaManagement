using API.Data;
using API.Extensions;
using ErrorOr;
using FluentValidation;
using MediatR;
using Shared.ServiceErrors;

namespace API.Features.Movies.CreateMovie;

public class CreateMovieRequestHandler(
    CinemaDbContext context,
    IValidator<CreateMovieRequest> validator,
    ILogger<CreateMovieRequestHandler> logger) : IRequestHandler<CreateMovieRequest, ErrorOr<CreateMovieResponse>>
{
    public async Task<ErrorOr<CreateMovieResponse>> Handle(CreateMovieRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Trying to add a new movie. Request: {0}", request);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.ToErrorList();
            logger.LogInformation("Validation failed for {0}. Errors: {1}", nameof(CreateMovieRequest),
                errors);
            return errors;
        }

        var movie = MovieMapper.ToMovieObject(request);

        var existingMovie = await context.Movies.FindAsync(movie.MovieId);
        if (existingMovie is not null)
        {
            logger.LogCritical("A movie with the same ID already exists. ID: {0}", movie.MovieId);
            return Errors.Movie.MovieAlreadyExists;
        }

        await context.Movies.AddAsync(movie, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Movie added to database. ID: {0}", movie.MovieId);
        return new CreateMovieResponse(movie.MovieId);
    }
}