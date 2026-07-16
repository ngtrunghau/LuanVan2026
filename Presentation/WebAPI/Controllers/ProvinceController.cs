using badmintion.Authorization;
using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers
{
   
    [Route("api/[controller]")]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.PROVINCE;
        public ProvinceController(BadmintionNlContext context, IProvinceService service, IHttpContextAccessor httpContextAccessor) 
        {
            _service = service;
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("get-all-core")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _service.GetAll();
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
