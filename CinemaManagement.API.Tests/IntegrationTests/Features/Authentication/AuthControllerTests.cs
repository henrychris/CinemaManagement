using System.Net;
using System.Net.Http.Json;
using API.Features.Authentication.Login;
using API.Features.Authentication.Register;
using API.Features.Authentication.Responses;
using Bogus;
using FluentAssertions;
using Shared.Responses;
using Shared.ServiceErrors;

namespace CinemaManagement.API.Tests.IntegrationTests.Features.Authentication;

public class AuthControllerTests : IntegrationTest
{
    [Test]
    public async Task Register_ValidRequestBody_ReturnsHttpOK()
    {
        // Arrange
        const string userRole = "User";

        var createUserRequest = new Faker<RegisterRequest>()
            .CustomInstantiator(f =>
                new RegisterRequest(
                    f.Person.FirstName,
                    f.Person.LastName,
                    f.Person.Email,
                    "testPassword12@",
                    userRole
                )).Generate();

        // Act
        var act = await TestClient.PostAsJsonAsync("Auth/register", createUserRequest);

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await act.Content.ReadFromJsonAsync<ApiResponse<UserAuthResponse>>();
        response!.Data.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.Data.Role.Should().Be(userRole);
        response.Data.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task Login_ValidRequestBody_ReturnsHttpOk()
    {
        // Arrange
        await RegisterAsync();
        var loginRequest = new LoginRequest(AuthEmailAddress, AuthPassword);

        // Act
        var act = await TestClient.PostAsJsonAsync("Auth/Login", loginRequest);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await act.Content.ReadFromJsonAsync<ApiResponse<UserAuthResponse>>();
        response!.Data.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Data!.AccessToken.Should().NotBeNullOrWhiteSpace();
        response.Data.Id.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task Login_InValidPassword_ReturnsHttpUnauthorized()
    {
        // Arrange
        await RegisterAsync();
        var loginRequest = new LoginRequest(AuthEmailAddress, "WrongPassword");

        // Act
        var act = await TestClient.PostAsJsonAsync("Auth/Login", loginRequest);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var response = await act.Content.ReadFromJsonAsync<ApiErrorResponse>();
        response!.Errors.Should().NotBeNull();
        response.Success.Should().BeFalse();
    }
    
    [Test]
    public async Task Login_ThreeFailedLogins_ReturnsHttpUnauthorizedAndLockedOut()
    {
        // Arrange
        await RegisterAsync();
        var loginRequest = new LoginRequest(AuthEmailAddress, "WrongPassword");

        // Act
        await TestClient.PostAsJsonAsync("Auth/Login", loginRequest);
        await TestClient.PostAsJsonAsync("Auth/Login", loginRequest);
        await TestClient.PostAsJsonAsync("Auth/Login", loginRequest);
        var act = await TestClient.PostAsJsonAsync("Auth/Login", loginRequest);

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var response = await act.Content.ReadFromJsonAsync<ApiErrorResponse>();
        response!.Errors[0].Code.Should().Be(Errors.User.IsLockedOut.Code);
        response.Errors.Should().NotBeNull();
        response.Success.Should().BeFalse();
    }
}