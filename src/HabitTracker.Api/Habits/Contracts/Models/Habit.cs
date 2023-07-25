namespace HabitTracker.Api.Habits.Contracts.Models;

public class Habit
{
    public uint Id { get; set; }
    public string Name { get; set; }

    public HabitSettings Settings { get; set; }
}