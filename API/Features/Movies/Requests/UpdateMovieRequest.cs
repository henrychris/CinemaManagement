using ErrorOr;
using MediatR;

namespace API.Features.Movies.Requests;

public record UpdateMovieRequest(
    string MovieId,
    string? Title,
    string? Description,
    int? DurationInMinutes,
    DateTime? ReleaseDate,
    string[]? Genres,
    int? Rating,
    string? Director) : IRequest<ErrorOr<Updated>>;