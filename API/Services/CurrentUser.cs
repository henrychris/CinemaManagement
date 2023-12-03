using API.Models.Enums;
using API.Services.Contracts;

namespace API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtClaims.UserId)?.Value;

    public string? Email =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtClaims.Email)?.Value;

    public string? Role =>
        httpContextAccessor.HttpContext?.User.FindFirst(JwtClaims.Role)?.Value;
}