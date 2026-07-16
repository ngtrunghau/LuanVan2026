using badmintion.DTO;
using badmintion.Models;
using FluentValidation;

namespace badmintion.Validation
{
    public class UserValidation : AbstractValidator<UserDTO>
    {
        public UserValidation() {
            RuleFor(model => model.Name)
       .NotEmpty().WithMessage("Name không được để trống.")
       .NotNull().WithMessage("Name không được để null.")
       .OverridePropertyName(x => x.Name);

            RuleFor(model => model.UserName)
         .NotEmpty().WithMessage("UserName không được để trống.")
         .NotNull().WithMessage("UserName không được để null.")
         .OverridePropertyName(x => x.UserName);

            RuleFor(model => model.Password)
          .NotEmpty().WithMessage("Password không được để trống.")
          .NotNull().WithMessage("Password không được để null.")
          .OverridePropertyName(x => x.Password);

            RuleFor(model => model.UnitRoleId)
         .NotEmpty().WithMessage("UnitRoleId không được để trống.")
         .NotNull().WithMessage("UnitRoleId không được để null.")
         .OverridePropertyName(x => x.UnitRoleId);
        }
    }
}
