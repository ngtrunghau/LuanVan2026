using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class ControllerManageValidation : AbstractValidator<ControllerManageDTO>
    {
        public ControllerManageValidation() {
            RuleFor(model => model.Name)
      .NotEmpty().WithMessage("Name không được để trống.")
      .NotNull().WithMessage("Name không được để null.")
      .OverridePropertyName(x => x.Name);
        }
    }
}
