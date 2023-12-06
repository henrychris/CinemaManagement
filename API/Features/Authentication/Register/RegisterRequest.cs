using MediatR;
using ErrorOr;

namespace API.Features.Authentication.Register;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string EmailAddress,
    string Password,
    string Role) : IRequest<ErrorOr<UserAuthResponse>>;