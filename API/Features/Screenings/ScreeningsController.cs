using System.Net.Mime;
using API.Extensions;
using API.Features.Screenings.CreateScreening;
using API.Features.Screenings.DeleteScreening;
using API.Features.Screenings.GetAllScreenings;
using API.Features.Screenings.GetScreening;
using API.Features.Screenings.UpdateScreening;
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

    [Authorize(Roles = UserRoles.Admin)]
    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMovie([FromBody] UpdateScreeningRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(_ => NoContent(), ReturnErrorResponse);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteScreening(string id)
    {
        var result = await mediator.Send(new DeleteScreeningRequest(id));
        return result.Match(_ => NoContent(), ReturnErrorResponse);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("all")]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<GetScreeningResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllScreenings([FromQuery] GetAllScreeningsRequest request)
    {
        var result = await mediator.Send(request);

        // If successful, return the event data in an ApiResponse.
        // If an error occurs, return an error response using the ReturnErrorResponse method.
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }
}