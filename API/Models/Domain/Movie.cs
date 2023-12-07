using System.ComponentModel.DataAnnotations;
using System.Text;
using Shared;

namespace API.Models.Domain;

public class Movie
{
    public Movie(string title,
        string description,
        int durationInMinutes,
        DateTime releaseDate,
        string[] genres,
        int rating,
        string director)
    {
        MovieId = GenerateMovieId(title, releaseDate);
        Title = title;
        Description = description;
        Director = director;
        DurationInMinutes = durationInMinutes;
        Rating = rating;
        Genres = genres;
        ReleaseDate = releaseDate;
    }

    public Movie()
    {
    }

    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public string MovieId { get; init; }

    [MaxLength(DomainConstants.MaxLength)] public string Title { get; set; }
    [MaxLength(DomainConstants.MaxLength)] public string Description { get; set; }
    [MaxLength(DomainConstants.MaxLength)] public string Director { get; set; }

    public int DurationInMinutes { get; set; }
    public int Rating { get; set; }
    public string[] Genres { get; set; }
    public DateTime ReleaseDate { get; set; }

    // navigation property
    public List<Screening> Screenings { get; set; } = [];

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