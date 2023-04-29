using HabitTrackerAPI.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTrackerAPI.Data.DataModelConfigurations;

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