using API.Extensions;
using API.Features.Theaters.CreateTheater;
using API.Features.Theaters.GetAllTheaters;
using API.Features.Theaters.GetSingleTheater;
using API.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.API;
using Shared.Responses;

namespace API.Features.Theaters;

public class TheatersController(IMediator mediator) : BaseController
{
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CreateTheaterResponse>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> CreateTheater([FromBody] CreateTheaterRequest request)
    {
        var result = await mediator.Send(request);

        return result.Match(
            response => CreatedAtAction(nameof(GetSingleTheater),
                routeValues: new { id = response.TheaterId },
                result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetTheaterResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSingleTheater(string id)
    {
        var result = await mediator.Send(new GetSingleTheaterRequest(id));
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }


    [Authorize(Roles = UserRoles.Admin)]
    [HttpGet("all")]
    [ProducesResponseType(typeof(PagedResponse<GetTheaterResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTheaters([FromQuery] GetAllTheatersRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }
}