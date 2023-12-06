using API.Features.Theaters.Requests;
using API.Features.Theaters.Responses;
using API.Models.Domain;
using ErrorOr;

namespace API.Features.Theaters;

public static class TheaterMapper
{
    public static Theater ToTheaterObject(CreateTheaterRequest request, string generatedTheaterId)
    {
        return new Theater
        {
            TheaterId = generatedTheaterId,
            Name = generatedTheaterId,
            ScreenType = request.ScreenType,
            AvailableSeats = request.AvailableSeats
        };
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