using HabitTrackerAPI.Contracts.Models;
using HabitTrackerAPI.Data;
using HabitTrackerAPI.Data.DataModel;

namespace HabitTrackerAPI.Service;
public class HabitService : IHabitService
{
    private readonly ApplicationDbContext _applicationDbContext;

    
    public HabitService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
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

    public async ValueTask<Habit?> GetHabitAsync(int id, CancellationToken cancellationToken)
    {
        var habit = await _applicationDbContext.Habits.FindAsync(id, cancellationToken);

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

    public ValueTask<List<Habit>> GetHabitsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    public ValueTask<int> GetHabitCurrentStreakAsync(int id, CancellationToken cancellationToken) 
    {
        throw new NotImplementedException();
    }
    
    public ValueTask<List<DayInformation>> GetCalendarInformation(int id, CancellationToken cancellationToken) 
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> UpdateHabitAsync(Habit habit)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> UpdateHabitStatus(int id, DateTime dateTime)
    {
        throw new NotImplementedException();
    }
    
    public Task RemoveHabitAsync(int id)
    {
        throw new NotImplementedException();
    }
}