using HabitTracker.Api.Habits;
using HabitTracker.Api.Infrastructure.DateTime;
using HabitTracker.Api.Infrastructure.Token;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.ExampleFilters();
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "Habit tracker API", Version = "v1" });
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();
builder.Services.AddHabits(builder.Configuration);
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

var app = builder.Build();
app.ApplyMigrations();
app.UseSwagger();
app.UseSwaggerUI(config =>
{
    config.SwaggerEndpoint("/swagger/v1/swagger.json", "Habit Tracker v1");
    config.RoutePrefix = string.Empty;
});
app.UseHabitEndpoints();

app.Run();