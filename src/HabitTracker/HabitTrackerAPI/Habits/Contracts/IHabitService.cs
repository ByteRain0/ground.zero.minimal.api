using HabitTrackerAPI.Habits.Contracts.Models;

namespace HabitTrackerAPI.Habits.Contracts;

public interface IHabitService
{
    Task<int> AddHabitAsync(Habit habit);

    ValueTask<Habit?> GetHabitAsync(int habitId, CancellationToken cancellationToken);

    ValueTask<List<Habit>> GetHabitsAsync(CancellationToken cancellationToken);

    Task<Habit?> UpdateHabitAsync(Habit habit);

    Task RemoveHabitAsync(int habitId);

    ValueTask<int> GetHabitCurrentStreakAsync(int habitId, DateOnly currentDate, CancellationToken cancellationToken);

    ValueTask<int> GetHabitLongestStreakAsync(int habitId, CancellationToken cancellationToken);

    Task<bool> UpdateHabitStatus(int habitId, DateOnly date);

    ValueTask<List<DayInformation>> GetMonthlyCompletionStatus(int habitId, DateOnly specificDay, CancellationToken cancellationToken);
}