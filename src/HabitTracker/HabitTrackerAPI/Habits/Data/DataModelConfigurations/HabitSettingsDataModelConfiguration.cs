using HabitTrackerAPI.Habits.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTrackerAPI.Habits.Data.DataModelConfigurations;

public class HabitSettingsDataModelConfiguration : IEntityTypeConfiguration<HabitSettingsDataModel>
{
    public void Configure(EntityTypeBuilder<HabitSettingsDataModel> builder)
    {
        //
    }
}