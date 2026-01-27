using badmintion.DTO;
using badmintion.Models;
using FluentValidation;

namespace badmintion.Validation
{
    public class CategoriesValidation : AbstractValidator<CategoriesDTO>
    {
        public CategoriesValidation() {
            RuleFor(model => model.Name)
        .NotEmpty().WithMessage("Name không được để trống.")
        .NotNull().WithMessage("Name không được để null.")
        .OverridePropertyName(x => x.Name);

  
        }
    }
}
