using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using API.Models.Domain;
using API.Models.Enums;
using MediatR;
using ErrorOr;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.ServiceErrors;

namespace API.Features.Authentication.Login;

public class LoginRequestHandler(
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    IOptionsSnapshot<JwtSettings> options,
    ILogger<LoginRequestHandler> logger) : IRequestHandler<LoginRequest, ErrorOr<UserAuthResponse>>
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<ErrorOr<UserAuthResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.EmailAddress);
        if (user is null)
        {
            logger.LogWarning("Email not found during login: {0}.", request.EmailAddress);
            return Errors.Auth.LoginFailed;
        }

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, true);
        if (signInResult.Succeeded)
        {
            logger.LogInformation("User {0} logged in successfully.", user.Id);
            return new UserAuthResponse
            {
                Id = user.Id,
                Role = user.Role,
                AccessToken = GenerateUserToken(user.Email!, user.Role, user.Id)
            };
        }

        if (signInResult.IsLockedOut)
        {
            logger.LogInformation("User {0} is locked out. End date: {1}.\n\tRequest: {2}", user.Id, user.LockoutEnd,
                JsonSerializer.Serialize(request));
            return Errors.User.IsLockedOut;
        }

        if (signInResult.IsNotAllowed)
        {
            logger.LogInformation("User {0} is not allowed to access the system.\n\tRequest: {1}", user.Id,
                JsonSerializer.Serialize(request));
            return Errors.User.IsNotAllowed;
        }

        logger.LogError("Login failed for user {0}.\n\tRequest: {1}", user.Id, JsonSerializer.Serialize(request));
        return Errors.Auth.LoginFailed;
    }

    private string GenerateUserToken(string emailAddress, string userRole, string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey!));

        var claims = new List<Claim>
        {
            new(JwtClaims.Email, emailAddress),
            new(JwtClaims.UserId, userId),
            new(JwtClaims.Role, userRole)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(_jwtSettings.TokenLifeTimeInHours)),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(claims)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}