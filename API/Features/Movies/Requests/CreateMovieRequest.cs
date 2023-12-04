using API.Features.Movies.Responses;
using ErrorOr;
using MediatR;

namespace API.Features.Movies.Requests;

public record CreateMovieRequest(
    string Title,
    string Description,
    int DurationInMinutes,
    DateTime ReleaseDate,
    string[] Genres,
    int Rating,
    string Director) : IRequest<ErrorOr<CreateMovieResponse>>;