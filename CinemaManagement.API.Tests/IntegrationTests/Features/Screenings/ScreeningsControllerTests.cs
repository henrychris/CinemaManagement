using System.Net;
using System.Net.Http.Json;
using API.Features.Movies.CreateMovie;
using API.Features.Screenings;
using API.Features.Screenings.CreateScreening;
using API.Features.Theaters.CreateTheater;
using API.Models.Enums;
using FluentAssertions;
using Shared.Responses;

namespace CinemaManagement.API.Tests.IntegrationTests.Features.Screenings;

public class ScreeningsControllerTests : IntegrationTest
{
    private static readonly string[] KotFmGenres = ["Crime", "Drama", "History"];
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
    private readonly CreateTheaterRequest _createStandardTheaterRequest = new(ScreenTypes.Standard.ToString(), 200);

    [Test]
    public async Task CreateScreening_ValidRequestReturns201()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var screeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(1),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var screeningRes = await screeningAct.Content.ReadFromJsonAsync<ApiResponse<CreateScreeningResponse>>();

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        screeningAct.EnsureSuccessStatusCode();

        screeningRes!.Data!.ScreeningId.Should().Contain(theaterRes.Data.TheaterId);
    }

    [Test]
    public async Task CreateScreening_WithSameStartTimeAsAnotherMovie_CollisionExists_ReturnsError()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var originalScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(1),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var duplicateScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(1),
            theaterRes.Data!.TheaterId,
            movieRes.Data!.MovieId
        ));

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        originalScreeningAct.EnsureSuccessStatusCode();

        duplicateScreeningAct.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task CreateScreening_WithinScreeningTimeOfAnotherMovie_CollisionExists_ReturnsError()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var originalScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(1),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var duplicateScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            // halfway through the previous screening
            DateTime.Now.AddHours(1).AddMinutes(CreateMovieRequest.DurationInMinutes / 2.0),
            theaterRes.Data!.TheaterId,
            movieRes.Data!.MovieId
        ));

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        originalScreeningAct.EnsureSuccessStatusCode();

        duplicateScreeningAct.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task CreateScreening_StartingWhenAnotherScreeningEnds_CollisionExists_ReturnsError()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var originalScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(1),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var duplicateScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            // starts immediately after the previous screening
            DateTime.Now.AddHours(1).AddMinutes(CreateMovieRequest.DurationInMinutes),
            theaterRes.Data!.TheaterId,
            movieRes.Data!.MovieId
        ));

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        originalScreeningAct.EnsureSuccessStatusCode();

        duplicateScreeningAct.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task CreateScreening_EndingWhenAnotherScreeningIsStillShowing_CollisionExists_ReturnsError()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var originalScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(5),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var duplicateScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            // starts before the other screening
            // ends when it is showing
            DateTime.Now.AddHours(2),
            theaterRes.Data!.TheaterId,
            movieRes.Data!.MovieId
        ));

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        originalScreeningAct.EnsureSuccessStatusCode();

        duplicateScreeningAct.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task CreateScreening_TenMinutesAfterScreeningTimeOfAnotherMovie_Returns201()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var originalScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(1),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var duplicateScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            // starts after the collision window after the previous screening
            DateTime.Now.AddHours(1).AddMinutes(CreateMovieRequest.DurationInMinutes).AddMinutes(10),
            theaterRes.Data!.TheaterId,
            movieRes.Data!.MovieId
        ));

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        originalScreeningAct.EnsureSuccessStatusCode();

        duplicateScreeningAct.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Test]
    public async Task CreateScreening_WhereEndTimeMatchesStartTimeOfAnotherMovie_ReturnsErrors()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var originalScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(5),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var duplicateScreeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            // starts before the other screening
            // ends when it is scheduled to start
            DateTime.Now.AddHours(5).AddMinutes(-CreateMovieRequest.DurationInMinutes),
            theaterRes.Data!.TheaterId,
            movieRes.Data!.MovieId
        ));

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        originalScreeningAct.EnsureSuccessStatusCode();

        duplicateScreeningAct.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }


    [Test]
    public async Task GetScreening_ScreeningExists_ReturnsHttpOk()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var theaterAct = await TestClient.PostAsJsonAsync("Theaters", _createStandardTheaterRequest);
        var movieAct = await TestClient.PostAsJsonAsync("Movies", CreateMovieRequest);

        var theaterRes = await theaterAct.Content.ReadFromJsonAsync<ApiResponse<CreateTheaterResponse>>();
        var movieRes = await movieAct.Content.ReadFromJsonAsync<ApiResponse<CreateMovieResponse>>();

        var screeningAct = await TestClient.PostAsJsonAsync("Screenings", new CreateScreeningRequest
        (
            DateTime.Now.AddHours(1),
            theaterRes!.Data!.TheaterId,
            movieRes!.Data!.MovieId
        ));
        var screeningRes = await screeningAct.Content.ReadFromJsonAsync<ApiResponse<CreateScreeningResponse>>();

        var getScreeningAct = await TestClient.GetAsync($"Screenings/{screeningRes!.Data!.ScreeningId}");
        var getScreeningRes = await getScreeningAct.Content.ReadFromJsonAsync<ApiResponse<GetScreeningResponse>>();

        // Assert
        theaterAct.EnsureSuccessStatusCode();
        movieAct.EnsureSuccessStatusCode();
        screeningAct.EnsureSuccessStatusCode();
        getScreeningAct.EnsureSuccessStatusCode();

        screeningRes.Data!.ScreeningId.Should().Be(getScreeningRes!.Data!.ScreeningId);
    }

    [Test]
    public async Task GetScreening_ScreeningDoesNotExist_ReturnsHttpNotFound()
    {
        // Arrange
        await AuthenticateAsync(UserRoles.Admin);

        // Act
        var getAct = await TestClient.GetAsync($"Screenings/notARealId");
        var getRes = await getAct.Content.ReadFromJsonAsync<ApiResponse<GetScreeningResponse>>();

        // Assert
        getAct.StatusCode.Should().Be(HttpStatusCode.NotFound);
        getRes!.Success.Should().BeFalse();
    }
}