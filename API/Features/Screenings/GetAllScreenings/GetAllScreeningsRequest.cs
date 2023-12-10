using ErrorOr;
using MediatR;
using Shared.Requests;
using Shared.Responses;

namespace API.Features.Screenings.GetAllScreenings;

public class GetAllScreeningsRequest : QueryStringParameters, IRequest<ErrorOr<PagedResponse<GetScreeningResponse>>>
{
}