using FluentValidation;
using HabitTrackerAPI.Habits.Contracts.Models;

namespace HabitTrackerAPI.Habits.Contracts.Validators;
public class HabitValidator : AbstractValidator<Habit>
{
    public HabitValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}