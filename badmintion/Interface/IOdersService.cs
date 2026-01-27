using badmintion.DTO;
using badmintion.Lib.Core;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface IOdersService
    {
        Task<dynamic> Create(OrdersDTO model);
        Task<dynamic> Update(OrdersDTO model);
        Task<dynamic> GetPagingCore(PagingParamDefault model); 
        Task<dynamic> GetPagingCoreStatus2(PagingParamDefault model);
        Task<dynamic> GetPagingCoreStatus3(PagingParamDefault model);
        Task<dynamic> GetPagingCoreStatus3Customer(PagingParamDefault model);
        Task<dynamic> GetIdCore(int id);

    }
}
