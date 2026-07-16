using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.CustomerController
{
    [Route("api/Customer/Categories")]
    public class CustomerCategoriesController : DefaultReposityController<Category>
    {
        private readonly ICategoriesService _service;
        public CustomerCategoriesController(BadmintionNlContext context, ICategoriesService service, IHttpContextAccessor httpContextAccessor) : base(context, DefaultNameCollection.CATEGORIES, httpContextAccessor)
        {
            _service = service;
        }
        
        [HttpGet]
        [Route("get-all-core")]
        public override async Task<IActionResult> GetAllData()
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
