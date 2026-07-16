using badmintion.DTO;

namespace badmintion.Interface
{
    public interface IUnitRoleService
    {
        Task<dynamic> Create(UnitRoleDTO model);
        Task<dynamic> Update(UnitRoleDTO model);
    }
}
