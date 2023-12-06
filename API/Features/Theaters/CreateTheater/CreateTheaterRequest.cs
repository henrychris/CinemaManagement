using ErrorOr;
using MediatR;

namespace API.Features.Theaters.CreateTheater;

public record CreateTheaterRequest(string ScreenType, int AvailableSeats) : IRequest<ErrorOr<CreateTheaterResponse>>;