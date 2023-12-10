using API.Data;
using ErrorOr;
using MediatR;

namespace API.Features.Screenings.DeleteScreening;

public class DeleteScreeningRequestHandler(CinemaDbContext context, ILogger<DeleteScreeningRequestHandler> logger)
    : IRequestHandler<DeleteScreeningRequest, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> Handle(DeleteScreeningRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}