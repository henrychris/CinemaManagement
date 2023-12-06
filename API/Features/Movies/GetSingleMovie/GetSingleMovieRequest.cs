using ErrorOr;
using MediatR;

namespace API.Features.Movies.GetSingleMovie;

public record GetSingleMovieRequest(string MovieId) : IRequest<ErrorOr<GetMovieResponse>>;