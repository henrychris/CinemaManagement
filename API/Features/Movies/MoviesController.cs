using System.Net.Mime;
using API.Extensions;
using API.Features.Movies.Requests;
using API.Features.Movies.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.API;
using Shared.Responses;

namespace API.Features.Movies;

public class MoviesController(IMediator mediator) : BaseController
{
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CreateMovieResponse>), StatusCodes.Status200OK)]
    // todo: authorise with admin role
    public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest request)
    {
        var result = await mediator.Send(request);

        return result.Match(
            response => CreatedAtAction(nameof(GetSingleMovie),
                routeValues: new GetSingleMovieRequest(response.MovieId),
                result.ToSuccessfulApiResponse()),
            ReturnErrorResponse);
    }

    [HttpGet("{id}")]
    [Produces(MediaTypeNames.Application.Json)]
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

    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> UpdateMovie([FromBody] UpdateMovieRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(_ => NoContent(), ReturnErrorResponse);
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<GetMovieResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteMovie(string id)
    {
        var result = await mediator.Send(new DeleteMovieRequest(id));
        return result.Match(_ => NoContent(), ReturnErrorResponse);
    }
}