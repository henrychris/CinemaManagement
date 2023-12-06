using API.Models.Enums;
using MediatR;
using Shared.Requests;
using ErrorOr;
using Shared.Responses;

namespace API.Features.Theaters.GetAllTheaters;

public class GetAllTheatersRequest : QueryStringParameters, IRequest<ErrorOr<PagedResponse<GetTheaterResponse>>>
{
    public string? Name { get; set; }
    public string? ScreenType { get; set; }
    public int? MinSeats { get; set; }
    public int? MaxSeats { get; set; }
}