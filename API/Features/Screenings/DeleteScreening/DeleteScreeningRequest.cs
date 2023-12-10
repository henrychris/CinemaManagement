using ErrorOr;
using MediatR;

namespace API.Features.Screenings.DeleteScreening;

public record DeleteScreeningRequest(string ScreeningId) : IRequest<ErrorOr<Deleted>>;