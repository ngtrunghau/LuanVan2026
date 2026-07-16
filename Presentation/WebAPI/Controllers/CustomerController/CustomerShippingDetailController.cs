using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Lib.Core;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.CustomerController
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Route("api/Customer/ShippingDetail")]
    public class CustomerShippingDetailController : DefaultReposityController<ShippingDetail>
    {
        private readonly IShippingDetailService _service;
        public CustomerShippingDetailController(BadmintionNlContext context, IShippingDetailService service, IHttpContextAccessor httpContextAccessor) : base(context, DefaultNameCollection.SHIPPING, httpContextAccessor)
        {
            _service = service;
        }
       
        [HttpPost]
        [Route("get-by-id-order")]
        public async Task<IActionResult> GetByIdOrder([FromBody] IdFromBodyModel model)
        {
            try
            {
                var response = await _service.GetByIdOrder(model.Id);
                return Ok(
                        new ResultMessageResponse()
                            .WithData(response)
                            .WithCode(DefaultCode.SUCCESS)
                            .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
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
        [Route("get-by-id-customer")]
        public async Task<IActionResult> GetByIdCustomer([FromBody] IdFromBodyModel model)
        {
            try
            {
                var response = await _service.GetByIdCustomer(model.Id);
                return Ok(
                        new ResultMessageResponse()
                            .WithData(response)
                            .WithCode(DefaultCode.SUCCESS)
                            .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
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
        public async Task<IActionResult> Update([FromBody] ShippingDetailDTO model)
        {
            try
            {
                if (!model.CustomerId.HasValue)
                {
                    return Ok(
                        new ResultMessageResponse()
                            .WithCode(DefaultCode.ERROR_STRUCTURE)
                            .WithMessage("Thiếu thông tin khách hàng xác nhận đơn.")
                    );
                }
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
    }
}
