using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.CustomerController
{
    [Route("api/Customer/[controller]")]
    public class AssistantController : ControllerBase
    {
        private readonly IAssistantService _service;

        public AssistantController(IAssistantService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("ask")]
        public async Task<IActionResult> Ask([FromBody] AssistantAskRequest model)
        {
            try
            {
                var response = await _service.Ask(model);
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
    }
}
