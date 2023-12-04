using System.ComponentModel.DataAnnotations;
using System.Text;
using Shared;

namespace API.Models.Domain;

public class Movie
{
    public Movie(string title, string description, int durationInMinutes, DateTime releaseDate, string[] genres, int rating, string director)
    {
        Title = title;
        Description = description;
        DurationInMinutes = durationInMinutes;
        ReleaseDate = releaseDate;
        Genres = genres;
        Director = director;
        Rating = rating;
        MovieId = GenerateMovieId(title, releaseDate);
    }

    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public required string MovieId { get; init; }

    [MaxLength(DomainConstants.MaxLength)] public required string Title { get; set; }
    [MaxLength(DomainConstants.MaxLength)] public required string Description { get; set; }
    [MaxLength(DomainConstants.MaxLength)] public required string Director { get; set; }

    public required int DurationInMinutes { get; set; }
    public required int Rating { get; set; }
    public required string[] Genres { get; set; }
    public required DateTime ReleaseDate { get; set; }

    // navigation property
    public List<ShowTime> ShowTimes { get; set; } = [];

    private string GenerateMovieId(string title, DateTime releaseDate)
    {
        var cleanTitle = RemoveSpecialCharacters(title);
        var formattedReleaseDate = releaseDate.ToString("yyMM");
        return $"{cleanTitle}-{formattedReleaseDate}";
    }

    private static string RemoveSpecialCharacters(string str)
    {
        var sb = new StringBuilder();
        foreach (var c in str)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}