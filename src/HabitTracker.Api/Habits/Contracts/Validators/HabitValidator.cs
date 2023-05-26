using FluentValidation;
using HabitTracker.Api.Habits.Contracts.Models;

namespace HabitTracker.Api.Habits.Contracts.Validators;
public class HabitValidator : AbstractValidator<Habit>
{
    public HabitValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}