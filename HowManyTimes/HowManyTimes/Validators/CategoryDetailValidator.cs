using HowManyTimes.Models;
using FluentValidation;

namespace HowManyTimes.Validators
{
    class CategoryDetailValidator : AbstractValidator<Category>
    {
        public CategoryDetailValidator()
        {
            RuleFor(x => x.Name).NotNull().MinimumLength(1).WithMessage("Category name must be entered!");
        }
    }
}
