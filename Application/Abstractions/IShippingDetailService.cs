using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface IShippingDetailService
    {
        Task<dynamic> Create(ShippingDetailDTO model);
        Task<dynamic> Update(ShippingDetailDTO model); 
        Task<dynamic> GetByIdOrder(int id);
        Task<dynamic> GetByIdCustomer(int id);
    }
}
