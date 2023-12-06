using ErrorOr;
using MediatR;

namespace API.Features.Movies.DeleteMovie;

public record DeleteMovieRequest(string MovieId) : IRequest<ErrorOr<Deleted>>;