namespace HabitTrackerAPI.Data.DataModel;

public class HabitDataDataModel
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }

    public int SettingsId { get; set; }
    
    public HabitSettingsDataModel Settings { get; set; }
    
    public IEnumerable<DayInformationDataModel> Days { get; set; }
}