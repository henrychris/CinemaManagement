using API.Features.Movies.Requests;
using API.Models.Domain;

namespace API.Features.Movies;

public static class MovieMapper
{
    public static Movie ToMovieObject(CreateMovieRequest request)
    {
        return new Movie(request.Title, request.Description, request.DurationInMinutes, request.ReleaseDate,
            request.Genres,
            request.Rating, request.Director);
    }
}