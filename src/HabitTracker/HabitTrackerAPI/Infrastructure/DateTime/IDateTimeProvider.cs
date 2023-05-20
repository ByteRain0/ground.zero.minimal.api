namespace HabitTrackerAPI.Infrastructure.DateTime;

// Replace with TimeProvider once we hit .NET 8
public interface IDateTimeProvider
{
    public System.DateTime CurrentTime();
}