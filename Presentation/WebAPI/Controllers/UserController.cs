using badmintion.Authorization;
using badmintion.Contansts;
using badmintion.Controllers.Core;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;

namespace badmintion.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : DefaultReposityController<User>
    {
        private readonly IUserService _service;
        private BadmintionNlContext _dataContext;
        private static string NameCollection = DefaultNameCollection.USER;
        public UserController(BadmintionNlContext context, IUserService service, IHttpContextAccessor httpContextAccessor) : base(context, NameCollection, httpContextAccessor)
        {
            _service = service;
        }
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create([FromBody] UserDTO model)
        {
            try
            {
                var response = await _service.Create(model);
                return Ok(
                        new ResultMessageResponse()
                            .WithData(response)
                            .WithCode(DefaultCode.SUCCESS)
                            .WithMessage(DefaultMessage.CREATE_SUCCESS)
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
        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> Update([FromBody] UserDTO model)
        {
            try
            {
                var response = await _service.Update(model);
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

        //[HttpPost]
        //[Route("get-by-id-core")]
        //public override async Task<IActionResult> GetById([FromBody] IdFromBodyModel model)
        //{
        //    try
        //    {
        //        var response = await _service.GetByIDCore(model.Id);
        //        return Ok(
        //                new ResultMessageResponse()
        //                    .WithData(response)
        //                    .WithCode(DefaultCode.SUCCESS)
        //                    .WithMessage(DefaultMessage.GET_DATA_SUCCESS)
        //            );
        //    }
        //    catch (ResponseMessageException ex)
        //    {
        //        return Ok(
        //            new ResultMessageResponse().WithCode(ex.ResultCode)
        //                .WithMessage(ex.ResultString).WithDetail(ex.Error)
        //        );
        //    }
        //}
    }
}
