using API.Features.Movies.Responses;
using ErrorOr;
using MediatR;

namespace API.Features.Movies.Requests;

public record GetSingleMovieRequest(string MovieId) : IRequest<ErrorOr<GetMovieResponse>>;