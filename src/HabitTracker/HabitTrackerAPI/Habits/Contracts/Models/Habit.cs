namespace HabitTrackerAPI.Habits.Contracts.Models;

public class Habit
{
    public int Id { get; set; }
    public string Name { get; set; }

    public HabitSettings Settings { get; set; }
}