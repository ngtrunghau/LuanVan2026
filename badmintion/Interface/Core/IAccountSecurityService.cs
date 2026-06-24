using badmintion.DTO;

namespace badmintion.Interface.Core
{
    public interface IAccountSecurityService
    {
        Task ChangePassword(ChangePasswordDTO model);
    }
}
