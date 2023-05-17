using HabitTrackerAPI.Habits.Contracts;
using HabitTrackerAPI.Habits.Contracts.Models;
using HabitTrackerAPI.Habits.Data;
using HabitTrackerAPI.Habits.Data.DataModel;
using Microsoft.EntityFrameworkCore;

namespace HabitTrackerAPI.Habits.Service;
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
    
    public ValueTask<int> GetHabitCurrentStreakAsync(int id, CancellationToken cancellationToken) 
    {
        throw new NotImplementedException();
    }
    
    public ValueTask<List<DayInformation>> GetCalendarInformation(int id, CancellationToken cancellationToken) 
    {
        throw new NotImplementedException();
    }
    
    public async Task<Habit?> UpdateHabitAsync(Habit model)
    {
        var habit = await _applicationDbContext.Habits.FirstOrDefaultAsync(x => x.Id == model.Id);

        if (habit == null)
        {
            return null;
        }

        habit.Name = model.Name;
        _applicationDbContext.Habits.Update(habit);
        await _applicationDbContext.SaveChangesAsync();

        return model;
    }
    
    public Task<bool> UpdateHabitStatus(int id, DateTime dateTime)
    {
        throw new NotImplementedException();
    }
    
    public async Task RemoveHabitAsync(int id)
    {
        // We already ensure the entity exists before removing it.
        // A new way to remove entities by id is on its way as far as I know so we will replace to that afterwards.
        _applicationDbContext.Habits.Remove((await _applicationDbContext.Habits.FindAsync(id))!);
        await _applicationDbContext.SaveChangesAsync();
    }
}