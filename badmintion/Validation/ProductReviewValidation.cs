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
          .NotNull().WithMessage("TotalStar không được để null.")
          .InclusiveBetween(1, 5).WithMessage("Số sao phải từ 1 đến 5.")
          .OverridePropertyName(x => x.TotalStar);

            RuleFor(model => model.OrderId)
          .NotNull().WithMessage("OrderId không được để null.")
          .GreaterThan(0).WithMessage("OrderId không hợp lệ.")
          .OverridePropertyName(x => x.OrderId);

        }
    }
}
