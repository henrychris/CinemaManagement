using API.Data;
using API.Features.Movies.Requests;
using API.Features.Movies.Responses;
using ErrorOr;
using MediatR;
using Shared.ServiceErrors;

namespace API.Features.Movies.Handlers;

public class GetSingleMovieRequestHandler(CinemaDbContext context, ILogger<GetSingleMovieRequestHandler> logger)
    : IRequestHandler<GetSingleMovieRequest, ErrorOr<GetMovieResponse>>
{
    public async Task<ErrorOr<GetMovieResponse>> Handle(GetSingleMovieRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching Movie with ID: {0}", request.MovieId);

        var movie = await context.Movies.FindAsync(request.MovieId);
        if (movie is null)
        {
            logger.LogError("Movie not found. ID: {0}", request.MovieId);
            return Errors.Movie.NotFound;
        }

        logger.LogInformation("Successfully fetched Movie with ID: {0}", request.MovieId);
        return MovieMapper.ToGetMovieResponse(movie);
    }
}