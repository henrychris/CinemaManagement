using API.Data;
using API.Models.Domain;
using API.Models.Enums;
using ErrorOr;
using MediatR;
using Shared.Responses;

namespace API.Features.Theaters.GetAllTheaters;

public class GetAllTheatersRequestHandler(CinemaDbContext context, ILogger<GetAllTheatersRequestHandler> logger)
    : IRequestHandler<GetAllTheatersRequest, ErrorOr<PagedResponse<GetTheaterResponse>>>
{
    public async Task<ErrorOr<PagedResponse<GetTheaterResponse>>> Handle(GetAllTheatersRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching theaters... \nRequest: {request}", request);

        var query = context.Theaters.AsQueryable();

        query = ApplyFilters(query, request);

        var pagedResults = query.Select(x => TheaterMapper.ToGetTheaterResponse(x));
        var response =
            await new PagedResponse<GetTheaterResponse>().ToPagedList(pagedResults, request.PageNumber, request.PageSize);

        logger.LogInformation("Fetched Theaters successfully. Returned {response.TotalCount} theaters.",
            response.TotalCount);
        return response;
    }

    private IQueryable<Theater> ApplyFilters(IQueryable<Theater> query, GetAllTheatersRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(x => x.Name.Contains(request.Name));
            logger.LogInformation("Filtered query by Name: {request.Name}.", request.Name);
        }

        if (!string.IsNullOrWhiteSpace(request.ScreenType))
        {
            Enum.TryParse<ScreenTypes>(request.ScreenType, out var screenType);
            query = query.Where(x => x.ScreenType == screenType.ToString());
            logger.LogInformation("Filtered query by screenType: {screenType}.", screenType.ToString());
        }

        if (request.MinSeats is not null)
        {
            query = query.Where(x => x.AvailableSeats >= request.MinSeats);
            logger.LogInformation("Filtered query by MinSeats: {request.MinSeats}.", request.MinSeats);
        }

        if (request.MaxSeats is not null)
        {
            query = query.Where(x => x.AvailableSeats <= request.MaxSeats);
            logger.LogInformation("Filtered query by MaxSeats: {request.MaxSeats}.", request.MaxSeats);
        }

        return query;
    }
}