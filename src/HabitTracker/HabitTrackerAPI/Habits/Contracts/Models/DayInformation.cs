namespace HabitTrackerAPI.Habits.Contracts.Models;

public class DayInformation
{
    public int HabitId { get; set; }

    public DateOnly Date { get; set; }

    public bool Checked { get; set; }
}