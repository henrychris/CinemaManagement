namespace API.Extensions;

public static class DateTimeExtensions
{
    public static string FormatScreeningTime(this DateTime dateTime)
    {
        return dateTime.ToString("dd-MMMM | HH:mm");
    }
}