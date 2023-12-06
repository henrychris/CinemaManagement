using MediatR;
using ErrorOr;

namespace API.Features.Theaters.GetSingleTheater;

public record GetSingleTheaterRequest(string TheaterId) : IRequest<ErrorOr<GetTheaterResponse>>;