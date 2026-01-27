using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Lib.Core;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Migrations;
using badmintion.Authorization;

namespace badmintion.Controllers
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class HistoryImportController : DefaultReposityController<HistoryImport>
    {
        private readonly IHistoryImportService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.HISTORY;
        public HistoryImportController(BadmintionNlContext context, IHistoryImportService service, IHttpContextAccessor httpContextAccessor) : base(context, NameCollection, httpContextAccessor)
        {
            _service = service;
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
    }
}
