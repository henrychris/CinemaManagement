using API.Data;
using ErrorOr;
using MediatR;

namespace API.Features.Screenings.UpdateScreening;

public class UpdateScreeningRequestHandler(CinemaDbContext context, ILogger<UpdateScreeningRequestHandler> logger)
    : IRequestHandler<UpdateScreeningRequest, ErrorOr<Updated>>
{
    public async Task<ErrorOr<Updated>> Handle(UpdateScreeningRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}