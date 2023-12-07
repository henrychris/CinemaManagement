using API.Features.Theaters.CreateTheater;
using API.Models.Domain;

namespace API.Features.Theaters;

public static class TheaterMapper
{
    public static Theater ToTheaterObject(CreateTheaterRequest request, string generatedTheaterId)
    {
        return new Theater
        (
            theaterId: generatedTheaterId,
            name: generatedTheaterId,
            screenType: request.ScreenType,
            availableSeats: request.AvailableSeats
        );
    }

    public static GetTheaterResponse ToGetTheaterResponse(Theater theater)
    {
        return new GetTheaterResponse(
            theater.TheaterId,
            theater.Name,
            theater.ScreenType,
            theater.AvailableSeats
        );
    }
}