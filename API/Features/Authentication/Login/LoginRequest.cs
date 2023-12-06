using ErrorOr;
using MediatR;

namespace API.Features.Authentication.Login;

public record LoginRequest(string EmailAddress, string Password) : IRequest<ErrorOr<UserAuthResponse>>;