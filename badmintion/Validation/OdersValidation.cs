using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class OdersValidation : AbstractValidator<OrdersDTO>
    {
        public OdersValidation() {
            RuleFor(model => model.TotalAmount)
        .NotEmpty().WithMessage("TotalAmount không được để trống.")
        .NotNull().WithMessage("TotalAmount không được để null.")
        .OverridePropertyName(x => x.TotalAmount);

            RuleFor(x => x)
                .Must(x => x.ListOrderItems != null)
                .OverridePropertyName("ListOrderItems")
                .WithMessage("ListOrderItems không được bỏ trống");

        }
    }
}
