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
    [Route("api/Customer/[controller]")]
    public class OdersController : DefaultReposityController<Order>
    {
        private readonly IOdersService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.ORDERS;
        public OdersController(BadmintionNlContext context, IOdersService service, IHttpContextAccessor httpContextAccessor) : base(context, NameCollection, httpContextAccessor)
        {
            _service = service;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] OrdersDTO model)
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
        public async Task<IActionResult> Update([FromBody] OrdersDTO model)
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
        [Route("get-paging-params-core")]
        public override async Task<IActionResult> GetPagingCore([FromBody] PagingParamDefault model)
        {
            try
            {
                var response = await _service.GetPagingCore(model);
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
        [Route("get-paging-params-core-status3")]
        public async Task<IActionResult> GetPagingCore3([FromBody] PagingParamDefault model)
        {
            try
            {
                var response = await _service.GetPagingCoreStatus3Customer(model);
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
