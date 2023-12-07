using API.Extensions;
using API.Features.Screenings.CreateScreening;
using API.Features.Screenings.GetScreening;
using API.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.API;
using Shared.Responses;

namespace API.Features.Screenings;

public class ScreeningsController(IMediator mediator) : BaseController
{
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CreateScreeningResponse>), StatusCodes.Status201Created)]
    [HttpPost]
    public async Task<IActionResult> CreateScreeningAsync([FromBody] CreateScreeningRequest request)
    {
        var result = await mediator.Send(request);

        return result.Match(
            response => CreatedAtAction(nameof(GetSingleScreening),
                routeValues: new { id = response.ScreeningId },
                result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetScreeningResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSingleScreening(string id)
    {
        var result = await mediator.Send(new GetSingleScreeningRequest(id));

        // If successful, return the event data in an ApiResponse.
        // If an error occurs, return an error response using the ReturnErrorResponse method.
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }
}