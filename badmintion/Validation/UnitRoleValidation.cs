using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class UnitRoleValidation : AbstractValidator<UnitRoleDTO>
    {
        public UnitRoleValidation() {
            RuleFor(model => model.Name)
       .NotEmpty().WithMessage("Name không được để trống.")
       .NotNull().WithMessage("Name không được để null.")
       .OverridePropertyName(x => x.Name);

        }
    }
}
