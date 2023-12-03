using System.ComponentModel.DataAnnotations;
using System.Text;
using Shared;

namespace API.Models.Domain;

public class Movie
{
    
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    // todo: use stolen code in mapper class to generate Id from name.
    public required string MovieId { get; set; }

    public required string Title { get; set; }
    public required string Description { get; set; }
    public required int DurationInMinutes { get; set; }
    public required string[] Genres { get; set; } = [];
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