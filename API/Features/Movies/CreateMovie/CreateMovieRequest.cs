using ErrorOr;
using MediatR;

namespace API.Features.Movies.CreateMovie;

public record CreateMovieRequest(
    string Title,
    string Description,
    int DurationInMinutes,
    DateTime ReleaseDate,
    string[] Genres,
    int Rating,
    string Director) : IRequest<ErrorOr<CreateMovieResponse>>;