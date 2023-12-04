using System.Net;
using System.Net.Http.Json;
using API.Features.Movies.Requests;
using API.Features.Movies.Responses;
using API.Models.Enums;
using FluentAssertions;
using Shared.Responses;

namespace CinemaManagement.API.Tests.IntegrationTests.Features.Movies;

public class MoviesControllerTests : IntegrationTest
{
    private static readonly string[] Genres = ["Crime", "Drama", "History"];

    private static readonly CreateMovieRequest CreateMovieRequest = new(
        "Killers Of The Flower Moon",
        "When oil is discovered in 1920s Oklahoma under Osage Nation land, the Osage people are murdered " +
        "one by one—until the FBI steps in to unravel the mystery.",
        206,
        new DateTime(2023, 10, 20),
        Genres,
        8,
        "Martin Scorsese"
    );

    [Test]
    public async Task CreateMovie_ValidRequest_ReturnsHttpOk()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var act = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);
        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var getAct = await TestClient.GetAsync($"Movies/{response!.Data!.MovieId}");
        var getRes = await getAct.Content.ReadFromJsonAsync<ApiResponse<GetMovieResponse>>();

        // Assert
        act.EnsureSuccessStatusCode();
        getAct.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.Created);

        response.Data.Should().NotBeNull();
        response.Data!.MovieId.Should().NotBeNull();
        response.Success.Should().BeTrue();

        getRes!.Data!.MovieId.Should().Be(response.Data.MovieId);
        getRes.Success.Should().BeTrue();
    }

    [Test]
    public async Task UpdateMovie_MovieDoesNotExist_ReturnsHttpNotFound()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var act = await TestClient.PutAsJsonAsync("Movies", new UpdateMovieRequest("doesntExist-2310"));
        var response = await act.Content.ReadFromJsonAsync<ApiErrorResponse>();

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response!.Success.Should().BeFalse();
    }

    [Test]
    public async Task UpdateMovie_MovieExistsAndValidDetails_ReturnsHttpNoContent()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);
        const string newTitle = "New Title";

        // Act
        var createAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);
        var createResponse = await createAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var updateAct =
            await TestClient.PutAsJsonAsync("Movies", new UpdateMovieRequest(createResponse!.Data!.MovieId, newTitle));

        // Assert
        createAct.EnsureSuccessStatusCode();
        createAct.StatusCode.Should().Be(HttpStatusCode.Created);

        updateAct.EnsureSuccessStatusCode();
        updateAct.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAct = await TestClient.GetAsync($"Movies/{createResponse.Data!.MovieId}");
        var getResponse = await getAct.Content.ReadFromJsonAsync<ApiResponse<GetMovieResponse>>();

        getResponse!.Data.Should().NotBeNull();
        getResponse.Data!.Title.Should().Be(newTitle);
        // original values should be unchanged
        getResponse.Data.Description.Should().Be(CreateMovieRequest.Description);
    }

    [Test]
    public async Task GetMovie_MovieDoesNotExist_ReturnsHttpNotFound()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var getAct = await TestClient.GetAsync($"Movies/notARealId");
        var getRes = await getAct.Content.ReadFromJsonAsync<ApiResponse<GetMovieResponse>>();

        // Assert
        getAct.StatusCode.Should().Be(HttpStatusCode.NotFound);
        getRes!.Success.Should().BeFalse();
    }

    [Test]
    public async Task GetMovie_MovieExists_ReturnsHttpOk()
    {
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var act = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);
        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        // Assert
        var getAct = await TestClient.GetAsync($"Movies/{response!.Data!.MovieId}");
        var getRes = await getAct.Content.ReadFromJsonAsync<ApiResponse<GetMovieResponse>>();

        // Assert
        act.EnsureSuccessStatusCode();
        getAct.EnsureSuccessStatusCode();
        getRes!.Success.Should().BeTrue();
        getRes.Data!.MovieId.Should().Be(response.Data.MovieId);
    }
}