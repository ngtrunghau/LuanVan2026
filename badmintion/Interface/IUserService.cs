using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface IUserService
    {
        Task<dynamic> Create(UserDTO model);
        Task<dynamic> Update(UserDTO model);
        Task<dynamic> GetByIDCore(int id);
 
    }
}
