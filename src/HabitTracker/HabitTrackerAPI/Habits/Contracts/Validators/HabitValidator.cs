using FluentValidation;
using HabitTrackerAPI.Contracts.Models;

namespace HabitTrackerAPI.Contracts.Validators;
public class HabitValidator : AbstractValidator<Habit>
{
    public HabitValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();
    }
}