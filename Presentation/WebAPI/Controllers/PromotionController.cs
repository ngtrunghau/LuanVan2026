using badmintion.Authorization;
using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class PromotionController : DefaultReposityController<Promotion>
    {
        private readonly IPromotionService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.PROMOTIONS;
        public PromotionController(BadmintionNlContext context, IPromotionService service, IHttpContextAccessor httpContextAccessor) : base(context, NameCollection, httpContextAccessor)
        {
            _service = service;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] Promotion model)
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
        public async Task<IActionResult> Update([FromBody] Promotion model)
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

        [HttpPost("paging")]
        public async Task<IActionResult> GetPaging([FromBody] PromotionPagingRequest model)
        {
            try
            {
                var response = await _service.GetPaging(model);
                return Ok(new ResultMessageResponse()
                    .WithData(response)
                    .WithCode(DefaultCode.SUCCESS)
                    .WithMessage(DefaultMessage.GET_DATA_SUCCESS));
            }
            catch (ResponseMessageException ex)
            {
                return Ok(new ResultMessageResponse()
                    .WithCode(ex.ResultCode)
                    .WithMessage(ex.ResultString)
                    .WithDetail(ex.Error));
            }
        }

        [HttpPost("set-active")]
        public async Task<IActionResult> SetActive([FromBody] PromotionStatusRequest model)
        {
            try
            {
                var response = await _service.SetActive(model);
                return Ok(new ResultMessageResponse()
                    .WithData(response)
                    .WithCode(DefaultCode.SUCCESS)
                    .WithMessage(model.IsActive
                        ? "Đã kích hoạt mã khuyến mãi."
                        : "Đã vô hiệu mã khuyến mãi."));
            }
            catch (ResponseMessageException ex)
            {
                return Ok(new ResultMessageResponse()
                    .WithCode(ex.ResultCode)
                    .WithMessage(ex.ResultString)
                    .WithDetail(ex.Error));
            }
        }
    }
}
