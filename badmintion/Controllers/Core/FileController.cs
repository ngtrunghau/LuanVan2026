using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.Core
{
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _service;
        public FileController(BadmintionNlContext context, IFileService service, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
        }

        [HttpPost]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [Route("upload")]
        public async Task<IActionResult> Upload(IFormFile files)
        {
            try
            {
                var data = await _service.Create(files);

                return Ok(
                    new ResultMessageResponse()
                        .WithData(data)
                        .WithResponse(DefaultCode.CREATE_SUCCESS));

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
