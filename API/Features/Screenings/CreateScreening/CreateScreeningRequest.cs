using ErrorOr;
using MediatR;

namespace API.Features.Screenings.CreateScreening;

public record CreateScreeningRequest(DateTime ScreeningDate, string TheaterId, string MovieId) : IRequest<ErrorOr<CreateScreeningResponse>>;