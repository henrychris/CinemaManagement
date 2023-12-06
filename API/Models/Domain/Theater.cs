using System.ComponentModel.DataAnnotations;
using API.Models.Enums;
using Shared;

namespace API.Models.Domain;

public class Theater
{
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public required string TheaterId { get; set; }

    [MaxLength(DomainConstants.MaxNameLength)]
    public required string Name { get; set; }
    [MaxLength(DomainConstants.MaxEnumLength)]
    public required string ScreenType { get; set; } = ScreenTypes.Standard.ToString();
    public required int AvailableSeats { get; set; }

    public List<ShowTime> ShowTimes { get; set; } = [];
}