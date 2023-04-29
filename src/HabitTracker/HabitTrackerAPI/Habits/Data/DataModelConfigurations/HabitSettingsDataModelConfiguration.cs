using HabitTrackerAPI.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTrackerAPI.Data.DataModelConfigurations;

public class HabitSettingsDataModelConfiguration : IEntityTypeConfiguration<HabitSettingsDataModel>
{
    public void Configure(EntityTypeBuilder<HabitSettingsDataModel> builder)
    {
        //
    }
}