using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface IAddressCustomerService
    {
        Task<dynamic> Create(AddressCustomerDTO model);
        Task<dynamic> Update(AddressCustomerDTO model);

        Task<dynamic> GetAllByIdUser(int id);
    }
}
