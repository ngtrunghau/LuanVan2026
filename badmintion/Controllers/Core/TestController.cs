using Azure;
using badmintion.Contansts;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Others;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET.Models;

namespace badmintion.Controllers.Core
{
    [Route("api/v1/[controller]")]
    public class TestController : Controller
    {
        private readonly IVnPayService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.ADDRESSCUSTOMER;
        public TestController(BadmintionNlContext context, IVnPayService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
        }

        [HttpPost]
        [Route("createUrl")]
        public async Task<IActionResult> Create([FromBody] MoneyPayment money, int id =1)
        {
            try
            {
                var response = await _service.CreatePaymentUrl(money.Money, id);
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
        [HttpGet("IpnAction")]
        public async Task<IActionResult> IpnAction()
        {
               try
                {
                if (Request.QueryString.HasValue)
                {
                    var paymentResult = _service.GetPaymentResult(Request.Query);
                    return Ok(
                        new ResultMessageResponse()
                            .WithData(paymentResult)
                            .WithCode(DefaultCode.SUCCESS)
                            .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
                    );
                }

                return Ok(
                         new ResultMessageResponse()
                             .WithData("")
                             .WithCode(DefaultCode.SUCCESS)
                             .WithMessage(DefaultMessage.GET_DATA_FAILURE)
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
        [HttpGet("Callback")]
        public async Task<dynamic> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = await _service.GetPaymentResult(Request.Query);
                    if (paymentResult == true)
                    {
                        return Redirect("http://localhost:8080/thanh-toan-thanh-cong");
                    }
                    return Redirect("http://localhost:8080/thanh-toan-that-bai");

                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}
