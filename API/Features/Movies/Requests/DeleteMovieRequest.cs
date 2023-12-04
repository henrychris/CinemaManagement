using ErrorOr;
using MediatR;

namespace API.Features.Movies.Requests;

public record DeleteMovieRequest(string MovieId) : IRequest<ErrorOr<Deleted>>;