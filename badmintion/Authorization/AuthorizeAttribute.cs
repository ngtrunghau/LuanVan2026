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
        private readonly BadmintionNlContext _context = null;
        private readonly DbSet<UnitRole> _unitRole;
        private readonly DbSet<FunctionManage> _functionManage;

        public AuthorizeAttribute()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            var connectionString = _configuration.GetConnectionString("dbconn");

            var services = new ServiceCollection();
            services.AddDbContext<BadmintionNlContext>(options =>
                options.UseSqlServer(connectionString));
            var serviceProvider = services.BuildServiceProvider();

            var _context = serviceProvider.GetRequiredService<BadmintionNlContext>();

            _functionManage = _context.FunctionManages;

        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            if (!ValidateToken(context.HttpContext.Request.Headers["Authorization"]))
            {
                context.Result = new JsonResult(
                    new ResultMessageResponse()
                        .WithCode(DefaultCode.BEYOND_TIME)
                        .WithMessage("Hết thời gian truy cập vui lòng đăng nhập lại để xử lý tiếp!")
                );
            }
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(context.HttpContext.Request.Headers["Authorization"]);
            var tokenS = jsonToken as JwtSecurityToken;
            int unitRole = Int32.Parse(tokenS.Claims.First(x => x.Type == ListActionDefault.UnitRoleIdString)?.Value);

            if (unitRole == null)
            {
                context.Result = new JsonResult(
                    new ResultMessageResponse()
                        .WithCode(DefaultCode.BEYOND_TIME)
                        .WithMessage("Hết thời gian truy cập vui lòng đăng nhập lại để xử lý tiếp!")
                );
            }
                

            //UnitRole data = _unitRole.Find(x => !x.IsDeleted && x.Id == unitRole).FirstOrDefault();
            List<FunctionManage> data = _functionManage.Where(x => x.UnitRoleId == unitRole && x.IsDeleted == false).Include(x => x.Controller).ToList();

            if (data != null && data.Count > 0)
            {
                var listAction = context.ActionDescriptor.AttributeRouteInfo.Template.Replace("api/v1/", "");

                var action = listAction.Split("/");

                if (action != null && action[0] != null && action[1] != null)
                {
                    foreach (var item in data)
                    {
                        var role = data.Find(x => x.Controller.Name == action[0] &&  x.Router == action[1]);
                        if (role == null)
                            context.Result = new JsonResult(
                                new ResultMessageResponse()
                                    .WithCode(DefaultCode.NOT_HAVE_ACCESS)
                                    .WithMessage(DefaultMessage.NOT_HAVE_ACCESS)
                            );
                    }
                  

                    
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

        private static bool ValidateToken(string authToken)
        {
            try
            {

                if (authToken == null || authToken == default)
                    return false;
                authToken = authToken.Replace("Bearer ", "");
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();
                SecurityToken validatedToken;
                IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
                var currentDate = DateTime.Now.ToLocalTime();
                var validatedLocal = validatedToken.ValidTo.ToLocalTime();
                if (validatedLocal < currentDate)
                {
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {

            }

            return false;
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("my@longshen#secretkey&05092019585954394348588")),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}
