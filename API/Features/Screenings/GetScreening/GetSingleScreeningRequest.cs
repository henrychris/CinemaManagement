using ErrorOr;
using MediatR;

namespace API.Features.Screenings.GetScreening;

public record GetSingleScreeningRequest(string ScreeningId) : IRequest<ErrorOr<GetScreeningResponse>>;