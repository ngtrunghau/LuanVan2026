using badmintion.DTO;

namespace badmintion.Interface
{
    public interface IWareHouseService
    {
        Task<dynamic> Create(WareHouseDTO model);
        Task<dynamic> Update(WareHouseDTO model);
    }
}
