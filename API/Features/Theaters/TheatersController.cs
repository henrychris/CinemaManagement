using API.Extensions;
using API.Features.Movies.Requests;
using API.Features.Theaters.Requests;
using API.Features.Theaters.Responses;
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

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetTheaterResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSingleTheater(string id)
    {
        var result = await mediator.Send(new GetSingleTheaterRequest(id));
        return result.Match(
            _ => Ok(result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }
}