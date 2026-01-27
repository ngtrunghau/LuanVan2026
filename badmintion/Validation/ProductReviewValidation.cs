using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class ProductReviewValidation : AbstractValidator<ProductReviewDTO>
    {
        public ProductReviewValidation() {
            RuleFor(model => model.ProductId)
         .NotEmpty().WithMessage("ProductId không được để trống.")
         .NotNull().WithMessage("ProductId không được để null.")
         .OverridePropertyName(x => x.ProductId);


            RuleFor(model => model.TotalStar)
          .NotEmpty().WithMessage("TotalStar không được để trống.")
          .NotNull().WithMessage("TotalStar không được để null.")
          .OverridePropertyName(x => x.TotalStar);


        }
    }
}
