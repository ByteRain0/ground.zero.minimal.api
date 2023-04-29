namespace HabitTrackerAPI.Data.DataModel;

public class DayInformationDataModel
{
    public int HabitId { get; set; }

    public HabitDataDataModel Habit { get; set; }

    public DateOnly Date { get; set; }

    public bool Checked { get; set; }
}