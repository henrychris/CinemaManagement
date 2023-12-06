namespace API.Features.Theaters;

public record GetTheaterResponse(
    string TheatreId,
    string Name,
    string ScreenType,
    int AvailableSeats
);