using badmintion.Interface;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class HistoryImportService : IHistoryImportService
    {
        private readonly BadmintionNlContext _context;

        public HistoryImportService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> GetPagingCore(PagingParamDefault model)
        {
            try
            {

                PagingModel<dynamic> result = new PagingModel<dynamic>();
                var data = await _context.HistoryImports.Where(x => x.IsDeleted == false).OrderByDescending(x => x.DateImport).Include(x => x.Product).Skip(model.Skip).Take(model.Limit).ToListAsync();

                result.Data = data;
                result.TotalRows = _context.HistoryImports.Where(x => x.IsDeleted == false).Count();
                return result;

            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }
    }
}
