using API.Features.Theaters.Responses;
using MediatR;
using ErrorOr;

namespace API.Features.Theaters.Requests;

public record GetSingleTheaterRequest(string TheaterId): IRequest<ErrorOr<GetTheaterResponse>>;