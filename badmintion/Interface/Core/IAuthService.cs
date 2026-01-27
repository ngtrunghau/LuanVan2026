using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface.Core
{
    public interface IAuthService
    {
        Task<dynamic> Login(AuthRequest files);
        Task<dynamic> LoginAsync(AuthRequest model);
        Task<User> AuthenticateTest(AuthRequest files);
        Task<dynamic> GenerateAuthenticationResultForUserAsync(User model);
    }
}
