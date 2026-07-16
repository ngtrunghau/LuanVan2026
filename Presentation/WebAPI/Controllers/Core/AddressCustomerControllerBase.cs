using badmintion.Contansts;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.Core;

/// <summary>
/// Shared address actions for the administration and customer API surfaces.
/// Route and authorization policies belong to the concrete controllers.
/// </summary>
public abstract class AddressCustomerControllerBase : DefaultReposityController<AddressCustomer>
{
    private readonly IAddressCustomerService _service;

    protected AddressCustomerControllerBase(
        BadmintionNlContext context,
        IAddressCustomerService service,
        IHttpContextAccessor httpContextAccessor)
        : base(context, DefaultNameCollection.ADDRESSCUSTOMER, httpContextAccessor)
    {
        _service = service;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] AddressCustomerDTO model)
    {
        try
        {
            var response = await _service.Create(model);
            return Ok(new ResultMessageResponse()
                .WithData(response)
                .WithCode(DefaultCode.SUCCESS)
                .WithMessage(DefaultMessage.CREATE_SUCCESS));
        }
        catch (ResponseMessageException ex)
        {
            return Ok(new ResultMessageResponse()
                .WithCode(ex.ResultCode)
                .WithMessage(ex.ResultString)
                .WithDetail(ex.Error));
        }
    }

    [HttpPost("update")]
    public async Task<IActionResult> Update([FromBody] AddressCustomerDTO model)
    {
        try
        {
            var response = await _service.Update(model);
            return Ok(new ResultMessageResponse()
                .WithData(response)
                .WithCode(DefaultCode.SUCCESS)
                .WithMessage(DefaultMessage.UPDATE_SUCCESS));
        }
        catch (ResponseMessageException ex)
        {
            return Ok(new ResultMessageResponse()
                .WithCode(ex.ResultCode)
                .WithMessage(ex.ResultString)
                .WithDetail(ex.Error));
        }
    }

    [HttpPost("get-address-by-id-user")]
    public async Task<IActionResult> GetByIdUser([FromBody] IdFromBodyModel model)
    {
        try
        {
            var response = await _service.GetAllByIdUser(model.Id);
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
}
