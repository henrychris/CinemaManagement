using System.ComponentModel.DataAnnotations;
using Shared;

namespace API.Models.Domain;

public class Seat
{
    // todo: add constructor to receive showtimeId and use to generate seatId
    // might need to use an ef core generator I think? i doubt.
    // create a range of seats using a for-loop or sumn, then AddRange()
    // seat number depends on the Id
    // SE-Nov-03-1430-001
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public required string SeatId { get; set; }

    public bool IsSeatReserved { get; set; }
    public required string SeatNumber { get; set; }

    [MaxLength(DomainConstants.MaxIdLength)]
    public required string TheatreId { get; set; }
    
    [MaxLength(DomainConstants.MaxIdLength)]
    public required string ScreeningId { get; set; }
}