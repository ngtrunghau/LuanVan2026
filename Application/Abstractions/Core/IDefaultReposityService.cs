using badmintion.Lib.Core;

namespace badmintion.Interface.Core
{
    public interface IDefaultReposityService<T> where T : class
    {
        Task<dynamic> GetAll();



        Task<dynamic> GetAllSelected();





        Task<dynamic> GetAllSelectedByCode(string code, string properties);


        Task<dynamic> GetById(IdFromBodyModel fromBodyModel);




        Task<bool> Delete(IdFromBodyModel fromBodyModel);

        Task<bool> Deleted(IdFromBodyModel fromBodyModel);



        Task<dynamic> GetPagingCore(PagingParamDefault pagingParam);








        Task<dynamic> GetAllPagingCore(PagingParamDefault pagingParam);






        Task<dynamic> GetListDataByProperties(IdFromBodyModel fromBodyModel, string properties);



        Task<dynamic> GetFirtstDataByProperties(IdFromBodyModel fromBodyModel, string properties);
    }
}
