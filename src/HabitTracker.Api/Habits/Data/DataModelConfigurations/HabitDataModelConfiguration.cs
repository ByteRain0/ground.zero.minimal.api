using HabitTracker.Api.Habits.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTracker.Api.Habits.Data.DataModelConfigurations;

public class HabitDataModelConfiguration : IEntityTypeConfiguration<HabitDataDataModel>
{
    
    public void Configure(EntityTypeBuilder<HabitDataDataModel> builder)
    {
        builder.Property(x => x.Name)
            .HasMaxLength(100);

        builder.HasOne(x => x.Settings)
            .WithOne(x => x.Habit)
            .HasForeignKey<HabitSettingsDataModel>(x => x.HabitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}