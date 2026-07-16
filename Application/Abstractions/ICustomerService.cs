using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface ICustomerService
    {
        Task<dynamic> Create(CustomersDTO model);
        Task<dynamic> Update(CustomersDTO model);
        Task<dynamic> Login(CustomersDTO model);
    }
}
