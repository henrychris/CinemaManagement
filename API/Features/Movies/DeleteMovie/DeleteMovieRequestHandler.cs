using API.Data;
using ErrorOr;
using MediatR;
using Shared.ServiceErrors;

namespace API.Features.Movies.DeleteMovie;

public class DeleteMovieRequestHandler(CinemaDbContext context, ILogger<DeleteMovieRequestHandler> logger)
    : IRequestHandler<DeleteMovieRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteMovieRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Received request to delete movie. ID: {0}", request.MovieId);

        var movie = await context.Movies.FindAsync(request.MovieId);
        if (movie is null)
        {
            logger.LogError("Movie not found. ID: {0}", request.MovieId);
            return Errors.Movie.NotFound;
        }

        context.Remove(movie);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully deleted Movie with ID: {0}", request.MovieId);
        return Result.Deleted;
    }
}