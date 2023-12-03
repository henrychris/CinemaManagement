using System.Net.Mime;
using API.Extensions;
using API.Features.Authentication.Login;
using API.Features.Authentication.Register;
using API.Features.Authentication.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.API;
using Shared.Responses;

namespace API.Features.Authentication;

public class AuthController(IMediator mediator) : BaseController
{
    [HttpPost("register")]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<UserAuthResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(_ => Ok(result.ToSuccessfulApiResponse()), ReturnErrorResponse);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<UserAuthResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
    {
        var result = await mediator.Send(request);
        return result.Match(_ => Ok(result.ToSuccessfulApiResponse()), ReturnErrorResponse);
    }
}