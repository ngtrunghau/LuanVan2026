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
          .NotNull().WithMessage("Giá trị giảm không được để trống.")
          .GreaterThan(0).WithMessage("Giá trị giảm phải lớn hơn 0.")
          .OverridePropertyName(x => x.DiscountValue);

            RuleFor(model => model.Code)
                .NotEmpty().WithMessage("Mã khuyến mãi không được để trống.");

            RuleFor(model => model.DiscountType)
                .Must(value =>
                    string.Equals(value, "percent", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(value, "fixed", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Loại giảm giá phải là percent hoặc fixed.");

            RuleFor(model => model.DiscountValue)
                .LessThanOrEqualTo(100)
                .When(model => string.Equals(
                    model.DiscountType,
                    "percent",
                    StringComparison.OrdinalIgnoreCase))
                .WithMessage("Mức giảm phần trăm không được vượt quá 100%.");

            RuleFor(model => model.MinOrderValue)
                .GreaterThanOrEqualTo(0)
                .When(model => model.MinOrderValue.HasValue)
                .WithMessage("Giá trị đơn tối thiểu không hợp lệ.");

            RuleFor(model => model.MaxUsage)
                .GreaterThan(0)
                .When(model => model.MaxUsage.HasValue)
                .WithMessage("Giới hạn lượt dùng phải lớn hơn 0.");

            RuleFor(model => model.EndDate)
                .GreaterThanOrEqualTo(model => model.StartDate)
                .When(model => model.StartDate.HasValue && model.EndDate.HasValue)
                .WithMessage("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");

        }
    }
}
