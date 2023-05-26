using HabitTracker.Api.Habits.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.Api.Habits.Data.DataModelConfigurations;

public class HabitSettingsDataModelConfiguration : IEntityTypeConfiguration<HabitSettingsDataModel>
{
    public void Configure(EntityTypeBuilder<HabitSettingsDataModel> builder)
    {
        //
    }
}