using badmintion.Lib.Core;

namespace badmintion.Interface
{
    public interface IHistoryImportService
    {
        Task<dynamic> GetPagingCore(PagingParamDefault model);
    }
}
