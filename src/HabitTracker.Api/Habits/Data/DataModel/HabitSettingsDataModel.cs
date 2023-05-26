namespace HabitTracker.Api.Habits.Data.DataModel;

public class HabitSettingsDataModel
{
    public int Id { get; set; }

    public int HabitId { get; set; }

    public HabitDataDataModel Habit { get; set; }
}