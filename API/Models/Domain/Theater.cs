using System.ComponentModel.DataAnnotations;
using API.Models.Enums;
using Shared;

namespace API.Models.Domain;

public class Theater
{
    // todo: name according to type, and order in DB.
    // e.g second IMAX screen would be TH-IMAX-2
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public required string TheaterId { get; set; }

    public required string Name { get; set; }
    public required string ScreenType { get; set; } = ScreenTypes.Standard;
    public required int AvailableSeats { get; set; }

    public List<ShowTime> ShowTimes { get; set; } = [];
}