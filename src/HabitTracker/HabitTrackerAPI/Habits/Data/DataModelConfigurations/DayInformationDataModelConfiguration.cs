using HabitTrackerAPI.Habits.Data.Convertors;
using HabitTrackerAPI.Habits.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTrackerAPI.Habits.Data.DataModelConfigurations;

public class DayInformationDataModelConfiguration : IEntityTypeConfiguration<DayInformationDataModel>
{
    public void Configure(EntityTypeBuilder<DayInformationDataModel> builder)
    {
        builder.Property(x => x.Date)
            .HasConversion<DateOnlyConverter>()
            .HasColumnType("date");

        builder.HasKey(x => new {x.HabitId, x.Date});
        
    }
}