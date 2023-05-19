namespace HabitTrackerAPI.Infrastructure;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime CurrentTime() => DateTime.UtcNow;
}