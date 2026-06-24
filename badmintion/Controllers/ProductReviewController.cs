using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers
{
    [Route("api/[controller]")]
    public class ProductReviewController : DefaultReposityController<ProductReview>
    {
        private readonly IProductReviewService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.PRODUCT_REVIEW;
        public ProductReviewController(BadmintionNlContext context, IProductReviewService service, IHttpContextAccessor httpContextAccessor) : base(context, NameCollection, httpContextAccessor)
        {
            _service = service;
        }
        [HttpPost]
        [Route("create")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Create([FromBody] ProductReviewDTO model)
        {
            try
            {
                var response = await _service.Create(model);
                return Ok(
                        new ResultMessageResponse()
                            .WithData(response)
                            .WithCode(DefaultCode.SUCCESS)
                            .WithMessage(DefaultMessage.CREATE_SUCCESS)
                    );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString).WithDetail(ex.Error)
                );
            }
        }
        [HttpPost]
        [Route("update")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Update([FromBody] ProductReviewDTO model)
        {
            try
            {
                var response = await _service.Update(model);
                return Ok(
                        new ResultMessageResponse()
                            .WithData(response)
                            .WithCode(DefaultCode.SUCCESS)
                            .WithMessage(DefaultMessage.UPDATE_SUCCESS)
                    );
            }
            catch (ResponseMessageException ex)
            {
                return Ok(
                    new ResultMessageResponse().WithCode(ex.ResultCode)
                        .WithMessage(ex.ResultString).WithDetail(ex.Error)
                );
            }
        }

        [HttpPost]
        [Route("moderation-list")]
        [badmintion.Authorization.Authorize]
        public async Task<IActionResult> GetModerationList([FromBody] ProductReviewFilterDTO model)
        {
            try
            {
                var response = await _service.GetModerationList(model);
                return Ok(new ResultMessageResponse()
                    .WithData(response)
                    .WithCode(DefaultCode.SUCCESS)
                    .WithMessage(DefaultMessage.GET_DATA_SUCCESS));
            }
            catch (ResponseMessageException ex)
            {
                return Ok(new ResultMessageResponse().WithCode(ex.ResultCode)
                    .WithMessage(ex.ResultString).WithDetail(ex.Error));
            }
        }

        [HttpPost]
        [Route("moderate")]
        [badmintion.Authorization.Authorize]
        public async Task<IActionResult> Moderate([FromBody] ProductReviewModerationDTO model)
        {
            try
            {
                var response = await _service.Moderate(model);
                return Ok(new ResultMessageResponse()
                    .WithData(response)
                    .WithCode(DefaultCode.SUCCESS)
                    .WithMessage(model.Status == 1 ? "Đã duyệt đánh giá." : "Đã ẩn đánh giá."));
            }
            catch (ResponseMessageException ex)
            {
                return Ok(new ResultMessageResponse().WithCode(ex.ResultCode)
                    .WithMessage(ex.ResultString).WithDetail(ex.Error));
            }
        }

        [HttpGet]
        [Route("approved-by-product/{productId:int}")]
        public async Task<IActionResult> GetApprovedByProduct(int productId)
        {
            try
            {
                var response = await _service.GetApprovedByProduct(productId);
                return Ok(new ResultMessageResponse()
                    .WithData(response)
                    .WithCode(DefaultCode.SUCCESS)
                    .WithMessage(DefaultMessage.GET_DATA_SUCCESS));
            }
            catch (ResponseMessageException ex)
            {
                return Ok(new ResultMessageResponse().WithCode(ex.ResultCode)
                    .WithMessage(ex.ResultString).WithDetail(ex.Error));
            }
        }
    }
}
