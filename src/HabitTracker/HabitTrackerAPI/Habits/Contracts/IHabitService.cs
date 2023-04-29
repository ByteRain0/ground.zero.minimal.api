using HabitTrackerAPI.Contracts.Models;

namespace HabitTrackerAPI.Service;

public interface IHabitService
{
    Task<int> AddHabitAsync(Habit habit);

    ValueTask<Habit?> GetHabitAsync(int id, CancellationToken cancellationToken);

    ValueTask<List<Habit>> GetHabitsAsync(CancellationToken cancellationToken);

    Task<bool> UpdateHabitAsync(Habit habit);

    Task RemoveHabitAsync(int id);

    ValueTask<int> GetHabitCurrentStreakAsync(int id, CancellationToken cancellationToken);
    
    Task<bool> UpdateHabitStatus(int id, DateTime dateTime);

    ValueTask<List<DayInformation>> GetCalendarInformation(int id, CancellationToken cancellationToken);
}