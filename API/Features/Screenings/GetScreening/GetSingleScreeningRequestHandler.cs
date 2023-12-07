using API.Data;
using API.Features.Screenings.CreateScreening;
using ErrorOr;
using MediatR;
using Shared.ServiceErrors;

namespace API.Features.Screenings.GetScreening;

public class GetSingleScreeningRequestHandler(CinemaDbContext context, ILogger<GetSingleScreeningRequestHandler> logger)
    : IRequestHandler<GetSingleScreeningRequest, ErrorOr<GetScreeningResponse>>
{
    public async Task<ErrorOr<GetScreeningResponse>> Handle(GetSingleScreeningRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching Screening with ID: {request.ScreeningId}", request.ScreeningId);

        var screening = await context.Screenings.FindAsync(request.ScreeningId);
        if (screening is null)
        {
            logger.LogError("Screening not found. ID: {request.ScreeningId}", request.ScreeningId);
            return Errors.Screening.NotFound;
        }

        logger.LogInformation("Successfully fetched Screening with ID: {request.ScreeningId}", request.ScreeningId);
        return ScreeningMapper.ToGetScreeningResponse(screening);
    }
}