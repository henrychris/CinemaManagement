using ErrorOr;
using MediatR;

namespace API.Features.Screenings.UpdateScreening;

public record UpdateScreeningRequest() : IRequest<ErrorOr<Updated>>;