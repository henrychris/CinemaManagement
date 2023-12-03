﻿using System.ComponentModel.DataAnnotations;
using Shared;

namespace API.Models.Domain;

public class ShowTime
{
    // todo: create a showtimeID
    // ST-MMDDTime
    // ST-Nov-03-1430
    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public required string ShowtimeId { get; set; }

    public DateTime ScreeningDate { get; set; }

    public Movie Movie { get; set; } = null!;

    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public required string MovieId { get; set; }

    public Theater Theater { get; set; } = null!;

    [Key, MaxLength(DomainConstants.MaxIdLength)]
    public required string TheatreId { get; set; }

    public List<Seat> Seats { get; set; } = [];
}