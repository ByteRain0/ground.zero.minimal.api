namespace HabitTracker.Api.Habits.Data.DataModel;

public class DayInformationDataModel
{
    public int HabitId { get; set; }

    public HabitDataDataModel Habit { get; set; }

    public DateOnly Date { get; set; }
}