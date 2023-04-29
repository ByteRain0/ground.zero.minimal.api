using HabitTrackerAPI.Data.DataModel;
using HabitTrackerAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HabitTrackerAPI.Data;

public class ApplicationDbContext : DbContext
{
    private readonly ITokenService _tokenService;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITokenService tokenService) :
        base(options)
    {
        _tokenService = tokenService;
    }

    public DbSet<HabitDataDataModel> Habits { get; set; }

    public DbSet<HabitSettingsDataModel> HabitSettings { get; set; }

    public DbSet<DayInformationDataModel> DaysInformation { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HabitDataDataModel>().HasQueryFilter(x => x.UserId == _tokenService.GetUserId());
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.SeedHabits();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entries = ChangeTracker.Entries();
        foreach (var entry in entries)
        {
            switch (entry.Entity)
            {
                case HabitDataDataModel trackable:
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            trackable.UserId = _tokenService.GetUserId();
                            break;
                        case EntityState.Modified:
                            // Update the data on modification
                            break;
                    }
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);    }
}