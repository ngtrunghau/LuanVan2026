using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class WareHouseValidation : AbstractValidator<WareHouseDTO>
    {
        public WareHouseValidation() {
            RuleFor(model => model.Name)
       .NotEmpty().WithMessage("Name không được để trống.")
       .NotNull().WithMessage("Name không được để null.")
       .OverridePropertyName(x => x.Name);

            RuleFor(model => model.ProductId)
          .NotEmpty().WithMessage("ProductId không được để trống.")
          .NotNull().WithMessage("ProductId không được để null.")
          .OverridePropertyName(x => x.ProductId);

            RuleFor(model => model.QuantityImport)
          .NotEmpty().WithMessage("QuantityImport không được để trống.")
          .NotNull().WithMessage("QuantityImport không được để null.")
          .OverridePropertyName(x => x.QuantityImport);


            RuleFor(model => model.RemainQuantity)
         .NotEmpty().WithMessage("RemainQuantity không được để trống.")
         .NotNull().WithMessage("RemainQuantity không được để null.")
         .OverridePropertyName(x => x.RemainQuantity);
        }
    }
}
