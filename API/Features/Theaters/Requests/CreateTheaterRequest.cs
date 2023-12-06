using API.Features.Theaters.Responses;
using ErrorOr;
using MediatR;

namespace API.Features.Theaters.Requests;

public record CreateTheaterRequest(string ScreenType, int AvailableSeats) : IRequest<ErrorOr<CreateTheaterResponse>>;