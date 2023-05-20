using HabitTrackerAPI.Habits.Contracts;
using HabitTrackerAPI.Habits.Contracts.Models;
using HabitTrackerAPI.Habits.Data;
using HabitTrackerAPI.Habits.Data.DataModel;
using HabitTrackerAPI.Infrastructure.DateTime;
using Microsoft.EntityFrameworkCore;

namespace HabitTrackerAPI.Habits.Service;

public class HabitService : IHabitService
{
    private readonly ApplicationDbContext _applicationDbContext;

    private readonly IDateTimeProvider _dateTimeProvider;

    public HabitService(ApplicationDbContext applicationDbContext, IDateTimeProvider dateTimeProvider)
    {
        _applicationDbContext = applicationDbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<int> AddHabitAsync(Habit model)
    {
        var habit = new HabitDataDataModel
        {
            Name = model.Name
        };
        await _applicationDbContext.Habits.AddAsync(habit);
        await _applicationDbContext.SaveChangesAsync();

        return habit.Id;
    }

    public async ValueTask<Habit?> GetHabitAsync(int habitId, CancellationToken cancellationToken)
    {
        var habit = await GetHabitAsync(habitId);

        if (habit is null)
        {
            return null;
        }

        return new Habit
        {
            Id = habit.Id,
            Name = habit.Name
        };
    }

    public async ValueTask<List<Habit>> GetHabitsAsync(CancellationToken cancellationToken)
    {
        //In future here you might add pagination

        var habits = await _applicationDbContext.Habits.ToListAsync(cancellationToken);

        //Might use mapper with projections
        return habits.Select(x => new Habit
        {
            Id = x.Id,
            Name = x.Name,
            Settings = new HabitSettings()
        }).ToList();
    }

    public async ValueTask<int> GetHabitCurrentStreakAsync(int habitId, DateOnly currentDate, CancellationToken cancellationToken)
    {
        var streakQuery = await _applicationDbContext.DaysInformation
            .Where(d => d.Date <= currentDate)
            .Where(d => d.HabitId == habitId)
            .OrderByDescending(d => d.Date)
            .ToListAsync(cancellationToken);
        
        var currentStreak = 0;

        if (streakQuery.Any())
        {
            // In case we have not checked the current day return 0;
            if (streakQuery.First().Date != currentDate) return 0;
        }
        
        for (var i = 0; i < streakQuery.Count; i++)
        {
            if (i > 0)
            {
                var previousDate = streakQuery[i - 1].Date.AddDays(-1);
                if (streakQuery[i].Date == previousDate)
                {
                    currentStreak++;
                }
                else
                {
                    break; // Exit the loop when streak is broken
                }
            }
            else
            {
                currentStreak++;
            }
        }
        

        return currentStreak;
    }

    public async ValueTask<int> GetHabitLongestStreakAsync(int habitId, CancellationToken cancellationToken)
    {
        var streakQuery = await _applicationDbContext
            .DaysInformation
            .Where(d => d.Date <= DateOnly.FromDateTime(_dateTimeProvider.CurrentTime()))
            .Where(d => d.HabitId == habitId)
            .OrderBy(d => d.Date)
            .ToListAsync(cancellationToken: cancellationToken);

        int longestStreak = 0;
        int currentStreak = 0;
        DateOnly previousDate = DateOnly.FromDateTime(DateTime.MinValue);

        foreach (var day in streakQuery)
        {
            if (day.Date == previousDate.AddDays(1))
            {
                currentStreak++;
            }
            else
            {
                if (currentStreak > longestStreak)
                {
                    longestStreak = currentStreak;
                }
                currentStreak = 1;
            }

            previousDate = day.Date;
        }

        // Check if the last streak is the longest
        if (currentStreak > longestStreak)
        {
            longestStreak = currentStreak;
        }

        return longestStreak;
    }

    public async ValueTask<List<DayInformation>> GetMonthlyCompletionStatus(int habitId, DateOnly specificDay,
        CancellationToken cancellationToken) =>
        await _applicationDbContext
            .DaysInformation
            //Somewhat of a magic number but it's just for PoC.
            .Where(x => x.Date >= new DateOnly(specificDay.Year, specificDay.Month, 1))
            .Where(x => x.HabitId == habitId)
            .Select(x => new DayInformation
                {
                    Date = x.Date,
                    HabitId = x.HabitId
                }
            ).ToListAsync(cancellationToken);

    public async Task<bool> UpdateHabitAsync(Habit model)
    {
        var habit = await GetHabitAsync(model.Id);

        if (habit is null)
        {
            return false;
        }

        habit.Name = model.Name;
        _applicationDbContext.Habits.Update(habit);
        await _applicationDbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveCompletedDay(int habitId, DateOnly date)
    {
        var habit = await GetHabitAsync(habitId);

        if (habit is null)
        {
            return false;
        }
        
        var completionEntry = await _applicationDbContext
            .DaysInformation
            .FirstOrDefaultAsync(x => x.HabitId == habitId && x.Date == date);

        if (completionEntry is null)
        {
            return true;
        }

        _applicationDbContext.Remove(completionEntry);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> AddCompletedDay(int habitId, DateOnly date)
    {
        var habit = await GetHabitAsync(habitId);

        if (habit is null)
        {
            return false;
        }
        
        var completionEntry = await _applicationDbContext
            .DaysInformation
            .FirstOrDefaultAsync(x => x.HabitId == habitId && x.Date == date);

        if (completionEntry is not null)
        {
            return true;
        }
        
        await _applicationDbContext.DaysInformation.AddAsync(new DayInformationDataModel
        {
            Date = date,
            HabitId = habitId
        });
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveHabitAsync(int habitId)
    {
        var habit = await GetHabitAsync(habitId);

        if (habit == null)
        {
            return false;
        }
        
        _applicationDbContext.Habits.Remove(habit);
        await _applicationDbContext.SaveChangesAsync();

        return true;
    }

    private async Task<HabitDataDataModel?> GetHabitAsync(int habitId)
    {
        var habit = await _applicationDbContext.Habits.FirstOrDefaultAsync(x => x.Id == habitId);
        return habit ?? null;
    }
}