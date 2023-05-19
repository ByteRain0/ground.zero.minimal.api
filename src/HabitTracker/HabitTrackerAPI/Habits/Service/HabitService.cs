using HabitTrackerAPI.Habits.Contracts;
using HabitTrackerAPI.Habits.Contracts.Models;
using HabitTrackerAPI.Habits.Data;
using HabitTrackerAPI.Habits.Data.DataModel;
using HabitTrackerAPI.Infrastructure;
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

    public async ValueTask<int> GetHabitCurrentStreakAsync(int habitId, CancellationToken cancellationToken)
    {
        var daysInformation = await _applicationDbContext.DaysInformation
            .Where(d => d.HabitId == habitId)
            .OrderByDescending(d => d.Date)
            .ToListAsync(cancellationToken);

        var currentStreakCount = 0;
        var counter = 0;

        while (daysInformation[counter].Checked)
        {
            currentStreakCount++;
            counter++;
        }

        return currentStreakCount;
    }

    public async ValueTask<int> GetHabitLongestStreakAsync(int habitId, CancellationToken cancellationToken)
    {
        var streakQuery =
            await _applicationDbContext
                .DaysInformation
                .Where(d => d.Checked)
                .Where(d => d.Date <= DateOnly.FromDateTime(_dateTimeProvider.CurrentTime()))
                .Where(d => d.HabitId == habitId)
                .OrderByDescending(d => d.Date)
                .ToListAsync(cancellationToken: cancellationToken);

        var longestStreak = 0;
        var currentStreak = 0;

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
                    if (currentStreak > longestStreak)
                    {
                        longestStreak = currentStreak;
                    }

                    currentStreak = 0;
                }
            }
            else
            {
                currentStreak++;
            }
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
            .Where(x => x.Date.Month == specificDay.Month && x.HabitId == habitId)
            .Select(x => new DayInformation
                {
                    Date = x.Date,
                    Checked = x.Checked,
                    HabitId = x.HabitId
                }
            ).ToListAsync(cancellationToken);

    public async Task<Habit?> UpdateHabitAsync(Habit model)
    {
        var habit = await GetHabitAsync(model.Id);

        if (habit is null)
        {
            return null;
        }

        habit.Name = model.Name;
        _applicationDbContext.Habits.Update(habit);
        await _applicationDbContext.SaveChangesAsync();

        return model;
    }

    public async Task<bool> UpdateHabitStatus(int habitId, DateOnly date)
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
            await _applicationDbContext.DaysInformation.AddAsync(new DayInformationDataModel
            {
                Date = date,
                HabitId = habitId,
                Checked = true
            });
        }
        else
        {
            completionEntry.Checked = !completionEntry.Checked;
            _applicationDbContext.DaysInformation.Update(completionEntry);
        }

        await _applicationDbContext.SaveChangesAsync();
        return true;
    }

    public async Task RemoveHabitAsync(int habitId)
    {
        // We already ensure the entity exists before removing it.
        // A new way to remove entities by id is on its way as far as I know so we will replace to that afterwards.
        _applicationDbContext.Habits.Remove((await _applicationDbContext.Habits.FindAsync(habitId))!);
        await _applicationDbContext.SaveChangesAsync();
    }

    private async Task<HabitDataDataModel?> GetHabitAsync(int habitId)
    {
        var habit = await _applicationDbContext.Habits.FirstOrDefaultAsync(x => x.Id == habitId);
        return habit ?? null;
    }
}