using System.Diagnostics.CodeAnalysis;
using API.Data;
using API.Features.Screenings.CreateScreening;
using API.Models.Domain;
using ErrorOr;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Shared.ServiceErrors;

namespace CinemaManagement.API.Tests.UnitTests.Features.Screenings;

[TestFixture]
[SuppressMessage("ReSharper", "EntityFramework.UnsupportedServerSideFunctionCall")]
public class CreateScreeningHandlerTest
{
    private const string TheaterId = "theaterId";
    private const string MovieId = "movieId";
    private readonly Mock<CinemaDbContext> _mockContext;
    private readonly CreateScreeningRequestHandler _handler;
    private readonly FakeTimeProvider _fakeTimeProvider;

    /// <summary>
    /// This needs to match the dates in the screenings below.
    /// </summary>
    private const string BaseDate = "2023-12-10";

    /// <summary>
    /// The screenings can be for different movies, or the same, it doesn't really matter.
    /// The point is two screenings should not clash in a theater.
    /// </summary>
    private readonly List<Screening> _screenings =
    [
        new Screening(DateTime.Parse("2023-12-10"), "Movie1", TheaterId, DateTime.Parse("2023-12-10T10:00:00"),
            DateTime.Parse("2023-12-10T12:00:00")),
        new Screening(DateTime.Parse("2023-12-10"), "Movie2", TheaterId, DateTime.Parse("2023-12-10T13:00:00"),
            DateTime.Parse("2023-12-10T15:00:00")),
        new Screening(DateTime.Parse("2023-12-10"), "Movie3", TheaterId, DateTime.Parse("2023-12-10T11:00:00"),
            DateTime.Parse("2023-12-10T13:30:00")),
    ];

    // [SetUp]
    public CreateScreeningHandlerTest()
    {
        _mockContext = new Mock<CinemaDbContext>();
        _fakeTimeProvider = new FakeTimeProvider();
        Mock<ILogger<CreateScreeningRequestHandler>> mockLogger = new();

        _handler = new CreateScreeningRequestHandler(
            _mockContext.Object,
            mockLogger.Object,
            _fakeTimeProvider
        );
    }

    [Test]
    public async Task CreateScreening_ValidationFails_ReturnsErrors()
    {
        // Arrange
        var createScreeningReq = new CreateScreeningRequest(DateTime.Parse(BaseDate).AddHours(-1), "theaterId", "movieId");
        _fakeTimeProvider.SetUtcNow(DateTime.Parse(BaseDate));

        // Act
        var result = await _handler.Handle(createScreeningReq, new CancellationToken());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Type.Should().Be(ErrorType.Validation);
    }

    [Test]
    public async Task CreateScreening_MovieIdDoesNotExist_ReturnsError()
    {
        // Arrange
        Movie? movie = null;
        _fakeTimeProvider.SetUtcNow(DateTime.Parse(BaseDate));

        // there's another screening in the list that starts at the same time as this
        var createScreeningReq = new CreateScreeningRequest(DateTime.Parse(BaseDate).AddHours(1), TheaterId, MovieId);
        _mockContext.Setup(x => x.Movies.FindAsync(MovieId)).ReturnsAsync(movie);

        // Act
        var result = await _handler.Handle(createScreeningReq, new CancellationToken());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Movie.NotFound);
    }

    [Test]
    public async Task CreateScreening_TheaterIdDoesNotExist_ReturnsError()
    {
        // Arrange
        Theater? theater = null;
        _fakeTimeProvider.SetUtcNow(DateTime.Parse(BaseDate));

        var createScreeningReq = new CreateScreeningRequest(DateTime.Parse(BaseDate).AddHours(1), TheaterId, MovieId);
        _mockContext.Setup(x => x.Movies.FindAsync(MovieId))
            .ReturnsAsync(new Movie());

        _mockContext.Setup(x => x.Theaters.FindAsync(TheaterId))
            .ReturnsAsync(theater);

        // Act
        var result = await _handler.Handle(createScreeningReq, new CancellationToken());

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(Errors.Theater.NotFound);
    }
}