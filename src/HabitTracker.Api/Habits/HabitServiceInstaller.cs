using FluentValidation;
using HabitTracker.Api.Habits.Contracts;
using HabitTracker.Api.Habits.Contracts.Validators;
using HabitTracker.Api.Habits.Data;
using HabitTracker.Api.Habits.Service;
using HabitTracker.Api.Infrastructure.Endpoints;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.Api.Habits;

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
    
    internal static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
        context?.Database.Migrate();
    }
}