using badmintion.Authorization;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _service;

        public AnalyticsController(IAnalyticsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("revenue-overview")]
        public async Task<IActionResult> GetRevenueOverview(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] DateTime? compareFromDate,
            [FromQuery] DateTime? compareToDate)
        {
            try
            {
                var response = await _service.GetRevenueOverview(fromDate, toDate, compareFromDate, compareToDate);
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
                        .WithMessage(ex.ResultString)
                        .WithDetail(ex.Error)
                );
            }
        }

        [HttpGet]
        [Route("revenue-trend")]
        public async Task<IActionResult> GetRevenueTrend(
            [FromQuery] string? groupBy,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var response = await _service.GetRevenueTrend(groupBy, fromDate, toDate);
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
                        .WithMessage(ex.ResultString)
                        .WithDetail(ex.Error)
                );
            }
        }

        [HttpGet]
        [Route("revenue-by-category")]
        public async Task<IActionResult> GetRevenueByCategory(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var response = await _service.GetRevenueByCategory(fromDate, toDate);
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
                        .WithMessage(ex.ResultString)
                        .WithDetail(ex.Error)
                );
            }
        }

        [HttpGet]
        [Route("revenue-by-payment-method")]
        public async Task<IActionResult> GetRevenueByPaymentMethod(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var response = await _service.GetRevenueByPaymentMethod(fromDate, toDate);
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
                        .WithMessage(ex.ResultString)
                        .WithDetail(ex.Error)
                );
            }
        }

        [HttpGet]
        [Route("inventory-insights")]
        public async Task<IActionResult> GetInventoryInsights(
            [FromQuery] int lookbackDays = 30,
            [FromQuery] int forecastDays = 14)
        {
            try
            {
                var response = await _service.GetInventoryInsights(lookbackDays, forecastDays);
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
                        .WithMessage(ex.ResultString)
                        .WithDetail(ex.Error)
                );
            }
        }

        [HttpGet("stock-alerts")]
        public async Task<IActionResult> GetStockAlerts(
            [FromQuery] bool includeAll = false,
            [FromQuery] int defaultThreshold = 5)
        {
            try
            {
                var response = await _service.GetStockAlerts(
                    includeAll,
                    defaultThreshold);
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

        [HttpPost("stock-threshold")]
        public async Task<IActionResult> SetStockThreshold(
            [FromBody] StockThresholdRequest model)
        {
            try
            {
                var response = await _service.SetStockThreshold(model);
                return Ok(new ResultMessageResponse()
                    .WithData(response)
                    .WithCode(DefaultCode.SUCCESS)
                    .WithMessage("Cập nhật ngưỡng cảnh báo thành công."));
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
