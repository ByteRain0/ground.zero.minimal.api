namespace HabitTrackerAPI.Infrastructure;

public interface IDateTimeProvider
{
    public DateTime CurrentTime();
}