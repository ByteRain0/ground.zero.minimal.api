using FluentValidation;
using HabitTrackerAPI.Contracts.Validators;
using HabitTrackerAPI.Data;
using HabitTrackerAPI.Habits;
using HabitTrackerAPI.Infrastructure;
using HabitTrackerAPI.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddValidatorsFromAssemblyContaining<HabitValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.ExampleFilters();
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "Habit tracker API", Version = "v1" });
});
builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(config =>
{
    config.SwaggerEndpoint("/swagger/v1/swagger.json", "Habit Tracker v1");
    config.RoutePrefix = string.Empty;
});

app.MapHabitEndpoints();

app.Run();