namespace HabitTrackerAPI.Infrastructure.DateTime;

public class DateTimeProvider : IDateTimeProvider
{
    public System.DateTime CurrentTime() => System.DateTime.UtcNow;
}