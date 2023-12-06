namespace API.Features.Movies;

public class GetMovieResponse
{
    public required string MovieId { get; set; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required int DurationInMinutes { get; init; }
    public required int Rating { get; init; }
    public required string Director { get; init; }
    public required string[] Genres { get; init; }
    public required string FormattedReleaseDate { get; init; }
}