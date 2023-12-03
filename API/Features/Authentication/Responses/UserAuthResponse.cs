namespace API.Features.Authentication.Responses;

public class UserAuthResponse
{
    public required string Id { get; init; }
    public required string Role { get; init; }
    public required string AccessToken { get; init; }
}