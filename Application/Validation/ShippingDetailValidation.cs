using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class ShippingDetailValidation : AbstractValidator<ShippingDetailDTO>
    {
        public ShippingDetailValidation() {
            RuleFor(model => model.Status)
       .NotEmpty().WithMessage("Status không được để trống.")
       .NotNull().WithMessage("Status không được để null.")
       .OverridePropertyName(x => x.Status);

            RuleFor(model => model.OrdersId)
          .NotEmpty().WithMessage("OrdersId không được để trống.")
          .NotNull().WithMessage("OrdersId không được để null.")
          .OverridePropertyName(x => x.OrdersId);

      
        }
    }
}
