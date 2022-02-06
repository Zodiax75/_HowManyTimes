using FluentValidation;
using HowManyTimes.Models;

namespace HowManyTimes.Validators
{
    internal class CounterDetailValidator : AbstractValidator<BaseCounter>
    {
        public CounterDetailValidator(bool stepActive)
        {
            if (stepActive)
                RuleFor(x => x.Step).GreaterThan(1).WithMessage("Step must be positive number");

            RuleFor(x => x.Name).NotEmpty().WithMessage("Counter name must be entered");
        }
    }
}
