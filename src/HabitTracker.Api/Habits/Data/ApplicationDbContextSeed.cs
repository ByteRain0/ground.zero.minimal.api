using HabitTracker.Api.Habits.Data.DataModel;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Api.Habits.Data;

public static class ApplicationDbContextSeed
{
    public static void SeedHabits(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HabitDataDataModel>()
            .HasData(new List<HabitDataDataModel>
            {
                new()
                {
                    Id = 1,
                    Name = ".NET Learning",
                    UserId = Guid.Empty.ToString()
                }
            });
    }
}