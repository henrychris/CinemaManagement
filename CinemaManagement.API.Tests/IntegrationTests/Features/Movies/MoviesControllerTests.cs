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

    [Test]
    public async Task CreateMovie_ValidRequest_ReturnsHttpOk()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        var createMovieRequest = new CreateMovieRequest(
            "Killers Of The Flower Moon",
            "When oil is discovered in 1920s Oklahoma under Osage Nation land, the Osage people are murdered " +
            "one by one—until the FBI steps in to unravel the mystery.",
            206,
            new DateTime(2023, 10, 20),
            Genres,
            8,
            "Martin Scorsese"
        );

        // Act
        var act = await TestClient.PostAsJsonAsync("Movies", createMovieRequest);

        // Assert
        act.EnsureSuccessStatusCode();
        act.StatusCode.Should().Be(HttpStatusCode.Created);

        var response = await act.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();
        response!.Data.Should().NotBeNull();
        response.Data!.MovieId.Should().NotBeNull();
        response.Success.Should().BeTrue();
    }
}