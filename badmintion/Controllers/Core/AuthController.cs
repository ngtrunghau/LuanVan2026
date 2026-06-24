using badmintion.Contansts;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Lib.Core;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;
using badmintion.DTO;

namespace badmintion.Controllers.Core
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _service;
        private BadmintionNlContext _dataContext;

        public AuthController(BadmintionNlContext context, IAuthService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Create([FromBody] AuthRequest model)
        {
            try
            {
                var response = await _service.LoginAsync(model);
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
