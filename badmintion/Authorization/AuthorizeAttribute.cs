using badmintion.Contansts;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Principal;
using System.Text;

namespace badmintion.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            var principal = context.HttpContext.User;
            if (principal.Identity?.IsAuthenticated != true)
            {
                context.Result = new JsonResult(
                    new ResultMessageResponse()
                        .WithCode(DefaultCode.BEYOND_TIME)
                        .WithMessage("Hết thời gian truy cập vui lòng đăng nhập lại để xử lý tiếp!")
                );
                return;
            }
            var roleValue = principal.Claims
                .FirstOrDefault(x => x.Type == ListActionDefault.UnitRoleIdString || x.Type == System.Security.Claims.ClaimTypes.Role)
                ?.Value;
            if (!int.TryParse(roleValue, out var unitRole))
            {
                context.Result = new JsonResult(
                    new ResultMessageResponse()
                        .WithCode(DefaultCode.BEYOND_TIME)
                        .WithMessage("Hết thời gian truy cập vui lòng đăng nhập lại để xử lý tiếp!")
                );
                return;
            }

            if (unitRole == ListActionDefault.UnitRoleId)
            {
                return;
            }
                

            var dbContext = context.HttpContext.RequestServices.GetRequiredService<BadmintionNlContext>();
            List<FunctionManage> data = dbContext.FunctionManages
                .Where(x => x.UnitRoleId == unitRole && x.IsDeleted == false)
                .Include(x => x.Controller)
                .ToList();

            if (data != null && data.Count > 0)
            {
                var listAction = context.ActionDescriptor.AttributeRouteInfo.Template.Replace("api/", "");

                var action = listAction.Split("/", StringSplitOptions.RemoveEmptyEntries);
                var controllerName = action.Length >= 2 ? action[^2] : action[0];
                var routerName = action.Length >= 1 ? action[^1] : string.Empty;

                if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrEmpty(routerName))
                {
                    var role = data.Find(x => x.Controller.Name == controllerName && x.Router == routerName);
                    if (role == null)
                        context.Result = new JsonResult(
                            new ResultMessageResponse()
                                .WithCode(DefaultCode.NOT_HAVE_ACCESS)
                                .WithMessage(DefaultMessage.NOT_HAVE_ACCESS)
                        );
                  

                    
                }

            }
            else
            {
                context.Result = new JsonResult(
                    new ResultMessageResponse()
                        .WithCode(DefaultCode.NOT_HAVE_ACCESS)
                        .WithMessage(DefaultMessage.NOT_HAVE_ACCESS)
                );
            }

        }

    }
}
