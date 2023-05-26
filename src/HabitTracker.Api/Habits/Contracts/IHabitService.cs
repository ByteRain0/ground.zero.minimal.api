using HabitTracker.Api.Habits.Contracts.Models;

namespace HabitTracker.Api.Habits.Contracts;

public interface IHabitService
{
    Task<int> AddHabitAsync(Habit habit);

    ValueTask<Habit?> GetHabitAsync(int habitId, CancellationToken cancellationToken);

    ValueTask<List<Habit>> GetHabitsAsync(CancellationToken cancellationToken);

    Task<bool> UpdateHabitAsync(Habit habit);

    Task<bool> RemoveHabitAsync(int habitId);

    ValueTask<int> GetHabitCurrentStreakAsync(int habitId, DateOnly currentDate, CancellationToken cancellationToken);

    ValueTask<int> GetHabitLongestStreakAsync(int habitId, CancellationToken cancellationToken);

    Task<bool> RemoveCompletedDay(int habitId, DateOnly date);

    Task<bool> AddCompletedDay(int habitId, DateOnly date);

    ValueTask<List<DayInformation>> GetMonthlyCompletionStatus(int habitId, DateOnly specificDay, CancellationToken cancellationToken);
}