using API.Data;
using API.Features.Theaters.Requests;
using API.Features.Theaters.Responses;
using ErrorOr;
using MediatR;
using Shared.ServiceErrors;

namespace API.Features.Theaters.Handlers;

public class GetSingleTheaterRequestHandler(CinemaDbContext context, ILogger<GetSingleTheaterRequestHandler> logger)
    : IRequestHandler<GetSingleTheaterRequest, ErrorOr<GetTheaterResponse>>
{
    public async Task<ErrorOr<GetTheaterResponse>> Handle(GetSingleTheaterRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching Theater with ID: {0}", request.TheaterId);

        var theater = await context.Theaters.FindAsync(request.TheaterId);
        if (theater is null)
        {
            logger.LogError("Theater not found. ID: {0}", request.TheaterId);
            return Errors.Theater.NotFound;
        }

        logger.LogInformation("Successfully fetched Theater with ID: {0}", request.TheaterId);
        return TheaterMapper.ToGetTheaterResponse(theater);
    }
}