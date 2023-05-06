using FluentValidation;
using HabitTrackerAPI.Data;
using HabitTrackerAPI.Habits.Contracts;
using HabitTrackerAPI.Habits.Contracts.Validators;
using HabitTrackerAPI.Habits.Data;
using HabitTrackerAPI.Habits.Service;
using HabitTrackerAPI.Infrastructure.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace HabitTrackerAPI.Habits;

public static class HabitServiceInstaller
{
    public static IServiceCollection AddHabits(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IHabitService, HabitService>();
        
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlServer(configuration.GetConnectionString("Default")));
        
        services.AddValidatorsFromAssemblyContaining<HabitValidator>();
        
        return services;
    }

    public static void UseHabitEndpoints(this IApplicationBuilder app)
    {
        app.UseEndpoints<HabitService>();
    }
}