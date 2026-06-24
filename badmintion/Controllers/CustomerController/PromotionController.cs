using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.CustomerController
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Route("api/Customer/[controller]")]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;

        public PromotionController(IPromotionService service)
        {
            _service = service;
        }

        [HttpPost("validate")]
        public async Task<IActionResult> Validate([FromBody] PromotionValidationRequest model)
        {
            var promotion = await _service.GetValidPromotion(model.Code, model.OrderValue);
            if (promotion == null)
            {
                return Ok(new ResultMessageResponse()
                    .WithCode(DefaultCode.DATA_NOT_FOUND)
                    .WithMessage("Mã khuyến mãi không hợp lệ hoặc đã hết hạn."));
            }

            var discount = _service.CalculateDiscount(promotion, model.OrderValue);
            return Ok(new ResultMessageResponse()
                .WithCode(DefaultCode.SUCCESS)
                .WithMessage("Áp dụng mã khuyến mãi thành công.")
                .WithData(new
                {
                    promotion.Code,
                    promotion.Descriptions,
                    DiscountAmount = discount
                }));
        }
    }
}
