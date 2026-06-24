using badmintion.DTO;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.Core
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountSecurityService _service;

        public AccountController(IAccountSecurityService service)
        {
            _service = service;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordDTO model)
        {
            try
            {
                await _service.ChangePassword(model);
                return Ok(new ResultMessageResponse()
                    .WithCode(DefaultCode.SUCCESS)
                    .WithMessage("Đổi mật khẩu thành công. Vui lòng đăng nhập lại."));
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
