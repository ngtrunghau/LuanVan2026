using badmintion.Authorization;
using badmintion.Interface;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers
{
 
    [Route("api/[controller]")]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictService _service;
        public DistrictController(BadmintionNlContext context, IDistrictService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("get-all-core")]
        public async Task<IActionResult> GetAll([FromBody]IdFromBodyModel model)
        {
            try
            {
                var response = await _service.GetAllByIdProvince(model.Id);
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
