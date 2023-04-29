using HabitTrackerAPI.Data.Convertors;
using HabitTrackerAPI.Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HabitTrackerAPI.Data.DataModelConfigurations;

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