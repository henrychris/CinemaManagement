using API.Features.Screenings.GetScreening;
using API.Models.Domain;
using ErrorOr;

namespace API.Features.Screenings.CreateScreening;

public static class ScreeningMapper
{
    public static Screening ToScreeningObject(CreateScreeningRequest request, int movieDurationInMinutes)
    {
        return new Screening(
            request.ScreeningDate,
            request.MovieId,
            request.TheaterId,
            request.ScreeningDate,
            request.ScreeningDate.AddMinutes(movieDurationInMinutes)
        );
    }

    public static GetScreeningResponse ToGetScreeningResponse(Screening screening)
    {
        return new GetScreeningResponse(
            screening.ShowtimeId,
            screening.MovieId,
            screening.TheatreId,
            screening.StartTime,
            screening.EndTime
        );
    }
}