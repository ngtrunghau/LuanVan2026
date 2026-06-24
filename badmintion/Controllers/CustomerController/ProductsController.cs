using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Lib.Core;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers.CustomerController
{
    [Route("api/Customer/[controller]")]
    public class ProductsController : DefaultReposityController<Product>
    {
        private readonly IProductService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.PRODUCTS;
        public ProductsController(BadmintionNlContext context, IProductService service, IHttpContextAccessor httpContextAccessor) : base(context, NameCollection, httpContextAccessor)
        {
            _service = service;
        }
        
        [HttpPost]
        [Route("get-product-by-id-category")]
        public async Task<IActionResult> GetProductByIdCate([FromBody] PagingParam model)
        {
            try
            {
                var response = await _service.GetByIdCate(model);
                return Ok(
                        new ResultMessageResponse()
                            .WithData(response)
                            .WithCode(DefaultCode.SUCCESS)
                            .WithMessage(DefaultMessage.UPDATE_SUCCESS)
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
