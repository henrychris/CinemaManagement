using System.ComponentModel.DataAnnotations;
using System.Text;
using Shared;

namespace API.Models.Domain;

public class Movie(
    string title,
    string description,
    int durationInMinutes,
    DateTime releaseDate,
    string[] genres,
    int rating,
    string director)
{
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public string MovieId { get; init; } = GenerateMovieId(title, releaseDate);

    [MaxLength(DomainConstants.MaxLength)] public string Title { get; set; } = title;
    [MaxLength(DomainConstants.MaxLength)] public string Description { get; set; } = description;
    [MaxLength(DomainConstants.MaxLength)] public string Director { get; set; } = director;

    public int DurationInMinutes { get; set; } = durationInMinutes;
    public int Rating { get; set; } = rating;
    public string[] Genres { get; set; } = genres;
    public DateTime ReleaseDate { get; set; } = releaseDate;

    // navigation property
    public List<ShowTime> ShowTimes { get; set; } = [];

    private static string GenerateMovieId(string title, DateTime releaseDate)
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