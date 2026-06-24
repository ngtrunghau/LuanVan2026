using badmintion.Interface.Core;
using System.Security.Claims;

namespace badmintion.Services.Core
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

        public bool IsCustomer =>
            string.Equals(
                Principal?.FindFirstValue("account_type"),
                "customer",
                StringComparison.OrdinalIgnoreCase);

        public int? UserId
        {
            get
            {
                var value = Principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? Principal?.FindFirstValue(ClaimTypes.Name);
                return int.TryParse(value, out var id) ? id : null;
            }
        }

        public string? UserName => Principal?.Identity?.Name;
    }
}
