using System.ComponentModel.DataAnnotations;
using API.Models.Enums;
using Shared;

namespace API.Models.Domain;

public class Theater
{
    public Theater()
    {
    }

    public Theater(string theaterId, string name, string screenType, int availableSeats)
    {
        TheaterId = theaterId;
        Name = name;
        ScreenType = screenType;
        AvailableSeats = availableSeats;
    }

    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public string TheaterId { get; set; }

    [MaxLength(DomainConstants.MaxNameLength)]
    public string Name { get; set; }
    [MaxLength(DomainConstants.MaxEnumLength)]
    public string ScreenType { get; set; } = ScreenTypes.Standard.ToString();
    public int AvailableSeats { get; set; }

    public List<Screening> Screenings { get; set; } = [];
}