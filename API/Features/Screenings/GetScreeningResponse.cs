namespace API.Features.Screenings;

public record GetScreeningResponse(
    string ScreeningId,
    string MovieId,
    string TheaterId,
    DateTime StartTime,
    DateTime EndTime);