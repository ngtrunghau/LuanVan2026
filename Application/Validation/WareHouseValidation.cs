using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class WareHouseValidation : AbstractValidator<WareHouseDTO>
    {
        public WareHouseValidation() {
            RuleFor(model => model.ProductId)
          .NotEmpty().WithMessage("ProductId không được để trống.")
          .NotNull().WithMessage("ProductId không được để null.")
          .OverridePropertyName(x => x.ProductId);

            RuleFor(model => model.QuantityImport)
          .GreaterThan(0).WithMessage("QuantityImport phải lớn hơn 0.")
          .OverridePropertyName(x => x.QuantityImport);
        }
    }
}
