using System.Net;
using System.Net.Http.Json;
using API.Features.Theaters;
using API.Features.Theaters.CreateTheater;
using API.Models.Enums;
using FluentAssertions;
using Shared.Responses;

namespace CinemaManagement.API.Tests.IntegrationTests.Features.Theaters;

public class TheatersControllerTests : IntegrationTest
{
    private readonly CreateTheaterRequest _createStandardTheaterRequest = new(ScreenTypes.Standard.ToString(), 200);
    private readonly CreateTheaterRequest _createAnotherStandardTheaterRequest = new(ScreenTypes.Standard.ToString(), 200);

    [Test]
    public async Task CreateTheater_ValidRequest_ReturnsHttpCreated()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);
        var expectedTheaterId = $"TH-{ScreenTypes.Standard}-1";

        // Act
        var act = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Data!.TheaterId.Should().Be(expectedTheaterId);
    }

    [Test]
    public async Task CreateTheater_CreateMultipleStandardTheaters_IdIncrementsCorrectly()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);
        var expectedTheaterId1 = $"TH-{ScreenTypes.Standard}-1";
        var expectedTheaterId2 = $"TH-{ScreenTypes.Standard}-2";

        // Act
        var act = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var duplicateAct = await TestClient.PostAsJsonAsync("Theaters", _createAnotherStandardTheaterRequest);
        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var duplicateResponse = await duplicateAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();

        // Assert
        act.EnsureSuccessStatusCode();
        duplicateAct.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.Created);
        duplicateAct.StatusCode.Should().Be(HttpStatusCode.Created);
        response!.Data!.TheaterId.Should().Be(expectedTheaterId1);
        duplicateResponse!.Data!.TheaterId.Should().Be(expectedTheaterId2);
    }

    [Test]
    public async Task GetTheater_TheaterExists_ReturnHttpOk()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var act = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();

        var getAct = await TestClient.GetAsync($"Theaters/{response!.Data!.TheaterId}");
        var getResponse = await getAct.Content.ReadFromJsonAsync<ApiResponse<GetTheaterResponse>>();

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.Created);
        getAct.EnsureSuccessStatusCode();
        getResponse!.Data!.TheatreId.Should().Be(response.Data.TheaterId);
    }

    [Test]
    public async Task GetTheater_TheaterDoesNotExist_ReturnNotFound()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var getAct = await TestClient.GetAsync($"Theaters/DoesNotExist");

        // Assert
        getAct.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}