using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Lib.Core;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;
using badmintion.Authorization;

namespace badmintion.Controllers
{
   
    [Route("api/[controller]")]
    public class TownController : ControllerBase
    {
        private readonly ITownService _service;
        public TownController(BadmintionNlContext context, ITownService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpPost]
        [Route("get-all-core")]
        public async Task<IActionResult> GetAll([FromBody] IdFromBodyModel model)
        {
            try
            {
                var response = await _service.GetAllByIdDistrict(model.Id);
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
