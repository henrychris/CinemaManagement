﻿using System.Net;
using System.Net.Http.Json;
using API.Features.Movies;
using API.Features.Movies.CreateMovie;
using API.Features.Movies.UpdateMovie;
using API.Models.Enums;
using FluentAssertions;
using Shared.Responses;

namespace CinemaManagement.API.Tests.IntegrationTests.Features.Movies;

public class MoviesControllerTests : IntegrationTest
{
    private static readonly string[] KotFmGenres = ["Crime", "Drama", "History"];
    private static readonly string[] OppGenres = ["Drama", "History"];

    private static readonly CreateMovieRequest CreateMovieRequest = new(
        "Killers Of The Flower Moon",
        "When oil is discovered in 1920s Oklahoma under Osage Nation land, the Osage people are murdered " +
        "one by one—until the FBI steps in to unravel the mystery.",
        206,
        new DateTime(2023, 10, 20),
        KotFmGenres,
        8,
        "Martin Scorsese"
    );

    private static readonly CreateMovieRequest CreateOtherMovieRequest = new(
        "Oppenheimer",
        "The story of J. Robert Oppenheimer's role in the development of the atomic bomb during World War II.",
        181,
        new DateTime(2023, 07, 21),
        OppGenres,
        8,
        "Christopher Nolan"
    );

    [Test]
    public async Task CreateMovie_ValidRequest_ReturnsHttpCreated()
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
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var act = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);
        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        // Assert
        var getAct = await TestClient.GetAsync($"Movies/{response!.Data!.MovieId}");
        var getRes = await getAct.Content.ReadFromJsonAsync<ApiResponse<GetMovieResponse>>();

        act.EnsureSuccessStatusCode();
        getAct.EnsureSuccessStatusCode();
        getRes!.Success.Should().BeTrue();
        getRes.Data!.MovieId.Should().Be(response.Data.MovieId);
    }

    [Test]
    public async Task DeleteMovie_MovieExists_ReturnsNoContent()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var act = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);
        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var deleteAct = await TestClient.DeleteAsync($"Movies/{response!.Data!.MovieId}");

        // Assert
        act.EnsureSuccessStatusCode();
        deleteAct.EnsureSuccessStatusCode();
        deleteAct.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeleteMovie_MovieDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var deleteAct = await TestClient.DeleteAsync($"Movies/MissingId");

        // Assert
        deleteAct.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetAllMovies_MoviesExist_ReturnsListOfMovies()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var act = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);
        var otherAct = await TestClient.PostAsJsonAsync("Movies", CreateOtherMovieRequest);

        var getAct = await TestClient.GetAsync("Movies/all");
        var getRes = await getAct.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<GetMovieResponse>>>();

        // Assert
        act.StatusCode.Should().Be(HttpStatusCode.Created);
        otherAct.StatusCode.Should().Be(HttpStatusCode.Created);
        getAct.EnsureSuccessStatusCode();
        getAct.StatusCode.Should().Be(HttpStatusCode.OK);
        getRes!.Success.Should().BeTrue();
        getRes.Data!.TotalCount.Should().Be(2);
    }
}