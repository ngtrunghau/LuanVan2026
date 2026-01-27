using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.CustomerController
{
    [Route("api/v1/Customer/[controller]")]
    public class CategoriesController : DefaultReposityController<Category>
    {
        private readonly ICategoriesService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.CATEGORIES;
        public CategoriesController(BadmintionNlContext context, ICategoriesService service, IHttpContextAccessor httpContextAccessor) : base(context, NameCollection, httpContextAccessor)
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
