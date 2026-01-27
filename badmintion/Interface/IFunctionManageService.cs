using badmintion.DTO;
using badmintion.Lib.Core;


namespace badmintion.Interface
{
    public interface IFunctionManageService
    {
        Task<dynamic> Create(FunctionManageDTO model);
        Task<dynamic> Update(FunctionManageDTO model); 
        Task<dynamic> GetPagingCore(PagingParamDefault model);
        //Task<object> GetListActionAPI();
    }
}
