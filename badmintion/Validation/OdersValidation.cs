using badmintion.DTO;
using FluentValidation;

namespace badmintion.Validation
{
    public class OdersValidation : AbstractValidator<OrdersDTO>
    {
        public OdersValidation() {
            RuleFor(x => x)
                .Must(x => x.ListOrderItems != null && x.ListOrderItems.Count > 0)
                .OverridePropertyName("ListOrderItems")
                .WithMessage("ListOrderItems không được bỏ trống");

            RuleForEach(x => x.ListOrderItems)
                .Must(x => x.ProductsId.HasValue && x.ProductsId.Value > 0
                           && x.Quantity.HasValue && x.Quantity.Value > 0)
                .WithMessage("Sản phẩm và số lượng đặt hàng không hợp lệ.");

        }
    }
}
