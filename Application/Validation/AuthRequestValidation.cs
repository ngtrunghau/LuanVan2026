using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class AuthRequestValidation : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidation() {
            RuleFor(model => model.Username)
          .NotEmpty().WithMessage("UserName không được để trống.")
          .NotNull().WithMessage("UserName không được để null.")
          .OverridePropertyName(x => x.Username);

            RuleFor(model => model.Password)
          .NotEmpty().WithMessage("Password không được để trống.")
          .NotNull().WithMessage("Password không được để null.")
          .OverridePropertyName(x => x.Password);
        }
    }
}
