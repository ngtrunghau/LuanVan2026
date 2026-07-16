using badmintion.DTO;

namespace badmintion.Interface
{
    public interface IControllerManageService
    {
        Task<dynamic> Create(ControllerManageDTO model);
        Task<dynamic> Update(ControllerManageDTO model);
    }
}
