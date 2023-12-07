using System.ComponentModel.DataAnnotations;
using Shared;

namespace API.Models.Domain;

public class Screening(DateTime screeningDate, string movieId, string theatreId, DateTime startTime, DateTime endTime)
{
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public string ShowtimeId { get; init; } = CreateScreeningId(theatreId, screeningDate);
    public DateTime ScreeningDate { get; set; } = screeningDate;

    [MaxLength(DomainConstants.MaxIdLength)]
    public string MovieId { get; init; } = movieId;

    [MaxLength(DomainConstants.MaxIdLength)]
    public string TheatreId { get; init; } = theatreId;

    public DateTime StartTime { get; set; } = startTime;
    public DateTime EndTime { get; set; } = endTime;

    public List<Seat> Seats { get; set; } = [];

    private static string CreateScreeningId(string theaterId, DateTime screeningDate)
    {
        var randomPart = Guid.NewGuid().ToString()[..4];
        return $"ST-{theaterId}-{screeningDate:yyyyMMM-ddHH}-{randomPart}";
    }
}