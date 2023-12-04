using System.Net.Mime;
using API.Extensions;
using API.Features.Movies.Requests;
using API.Features.Movies.Responses;
using API.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.API;
using Shared.Responses;

namespace API.Features.Movies;

public class MoviesController(IMediator mediator) : BaseController
{
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<CreateMovieResponse>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest request)
    {
        var result = await mediator.Send(request);

        return result.Match(
            response => CreatedAtAction(nameof(GetSingleMovie),
                routeValues: new { id = response.MovieId },
                result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetMovieResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSingleMovie(string id)
    {
        var result = await mediator.Send(new GetSingleMovieRequest(id));

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
    public async Task<IActionResult> UpdateMovie([FromBody] UpdateMovieRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(_ => NoContent(), ReturnErrorResponse);
    }

    [Authorize(Roles = UserRoles.Admin)]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMovie(string id)
    {
        var result = await mediator.Send(new DeleteMovieRequest(id));
        return result.Match(_ => NoContent(), ReturnErrorResponse);
    }
}