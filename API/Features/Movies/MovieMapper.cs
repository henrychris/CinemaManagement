using API.Features.Movies.CreateMovie;
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

    public static GetMovieResponse ToGetMovieResponse(Movie movie)
    {
        return new GetMovieResponse
        {
            MovieId = movie.MovieId,
            Title = movie.Title,
            Description = movie.Description,
            DurationInMinutes = movie.DurationInMinutes,
            Rating = movie.Rating,
            Director = movie.Director,
            Genres = movie.Genres,
            FormattedReleaseDate = movie.ReleaseDate.ToString("dd MMMM yyyy"),
        };
    }
}