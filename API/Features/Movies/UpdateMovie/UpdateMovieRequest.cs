using ErrorOr;
using MediatR;

namespace API.Features.Movies.UpdateMovie;

public record UpdateMovieRequest(
    string MovieId,
    string? Title = null,
    string? Description = null,
    int? DurationInMinutes = null,
    DateTime? ReleaseDate = null,
    string[]? Genres = null,
    int? Rating = null,
    string? Director = null) : IRequest<ErrorOr<Updated>>;