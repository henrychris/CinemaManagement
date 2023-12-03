using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Extensions;
using API.Features.Authentication.Responses;
using API.Models.Domain;
using API.Models.Enums;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.ServiceErrors;

namespace API.Features.Authentication.Register;

public class RegisterRequestHandler(
    UserManager<ApplicationUser> userManager,
    IOptionsSnapshot<JwtSettings> options,
    ILogger<RegisterRequestHandler> logger,
    IValidator<RegisterRequest> validator) : IRequestHandler<RegisterRequest, ErrorOr<UserAuthResponse>>
{
    private readonly JwtSettings _jwtSettings = options.Value;

    public async Task<ErrorOr<UserAuthResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Registration request received for email: {0}.", request.EmailAddress);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed for new user: {0}", request.EmailAddress);
            return validationResult.ToErrorList();
        }

        var user = await userManager.FindByEmailAsync(request.EmailAddress);
        if (user is not null)
        {
            logger.LogWarning("Duplicate email found during registration: {0}", request.EmailAddress);
            return Errors.User.DuplicateEmail;
        }

        var newUser = MapToApplicationUser(request);
        var result = await userManager.CreateAsync(newUser, request.Password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(newUser, newUser.Role);
            logger.LogInformation("User registered successfully: {0}.", request.EmailAddress);

            return new UserAuthResponse
            {
                Id = newUser.Id,
                Role = newUser.Role,
                AccessToken = GenerateUserToken(newUser.Email!, newUser.Role, newUser.Id)
            };
        }

        var errors = result.Errors
            .Select(error => Error.Validation("User." + error.Code, error.Description))
            .ToList();

        logger.LogError(
            "User registration failed for email: {0}.\nErrors: {1}", request.EmailAddress,
            string.Join(", ", errors.Select(e => $"{e.Code}: {e.Description}"))
        );

        return errors;
    }

    private static ApplicationUser MapToApplicationUser(RegisterRequest request)
    {
        return new ApplicationUser
        {
            FirstName = request.FirstName.FirstCharToUpper(),
            LastName = request.LastName.FirstCharToUpper(),
            Email = request.EmailAddress,
            UserName = request.EmailAddress,
            Role = request.Role
        };
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