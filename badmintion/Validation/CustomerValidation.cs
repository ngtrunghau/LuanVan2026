using badmintion.DTO;
using badmintion.Models;
using FluentValidation;

namespace badmintion.Validation
{
    public class CustomerValidation : AbstractValidator<CustomersDTO>
    {
        public CustomerValidation() {
            RuleFor(model => model.UserName)
           .NotEmpty().WithMessage("UserName không được để trống.")
           .NotNull().WithMessage("UserName không được để null.")
           .OverridePropertyName(x => x.UserName);

            RuleFor(model => model.Password)
          .NotEmpty().WithMessage("Password không được để trống.")
          .NotNull().WithMessage("Password không được để null.")
          .OverridePropertyName(x => x.Password);

            //  RuleFor(model => model.Phone)
            //.NotEmpty().WithMessage("Phone không được để trống.")
            //.NotNull().WithMessage("Phone không được để null.")
            //.OverridePropertyName(x => x.Phone);

            //  RuleFor(model => model.FullName)
            //.NotEmpty().WithMessage("FullName không được để trống.")
            //.NotNull().WithMessage("FullName không được để null.")
            //.OverridePropertyName(x => x.FullName);

        }
    }
}
