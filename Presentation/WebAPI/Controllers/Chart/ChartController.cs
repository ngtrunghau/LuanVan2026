using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Chart;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.Chart
{
    [Route("api/[controller]")]
    public class ChartController : ControllerBase
    {
        private readonly IChartService _service;
        public ChartController(BadmintionNlContext context, IChartService service, IHttpContextAccessor httpContextAccessor) 
        {
            _service = service;
        }
        [HttpGet]
        [Route("get-doanh-thu-thang")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var response = await _service.GetDoanhThuThang();
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
    }
}
