using badmintion.DTO;
using badmintion.Models;
using FluentValidation;

namespace badmintion.Validation
{
    public class PromotionValidation : AbstractValidator<Promotion>
    {
        public PromotionValidation() {
            RuleFor(model => model.Descriptions)
       .NotEmpty().WithMessage("Descriptions không được để trống.")
       .NotNull().WithMessage("Descriptions không được để null.")
       .OverridePropertyName(x => x.Descriptions);

            RuleFor(model => model.DiscountValue)
          .NotEmpty().WithMessage("DiscountValue không được để trống.")
          .NotNull().WithMessage("DiscountValue không được để null.")
          .OverridePropertyName(x => x.DiscountValue);

        }
    }
}
