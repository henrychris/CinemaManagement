namespace API.Features.Theaters.Responses;

public record GetTheaterResponse(
    string TheatreId,
    string Name,
    string ScreenType,
    int AvailableSeats
);