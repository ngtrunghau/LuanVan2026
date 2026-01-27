using badmintion.Interface.Core;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace badmintion.Services.Core
{
    public class DefaultReposityService<T> : IDefaultReposityService<T> where T : class
    {
        protected DbSet<T> _collection;

        private readonly IHttpContextAccessor _contextAccessor;
        protected readonly BadmintionNlContext _context;

        public DefaultReposityService(DbSet<T> collection, BadmintionNlContext dataContext, IHttpContextAccessor contextAccessor)
        {

            _context = dataContext;
            _collection = collection;
            _contextAccessor = contextAccessor;

        }

        public async Task<bool> Delete(IdFromBodyModel fromBodyModel)
        {
            try
            {
                dynamic data = await _context.Set<T>()
                   .Where(x => EF.Property<bool>(x, "IsDeleted") == false && EF.Property<int>(x, "Id") == fromBodyModel.Id) // Lọc theo IsDeleted và Id
                   .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên (hoặc null nếu không có)



                if (data == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);



                var propertyInfo = data.GetType().GetProperty("IsDeleted");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(data, true);
                }

                int result = await _context.SaveChangesAsync();

                if (result < 0)
                    throw new ResponseMessageException().WithException(DefaultCode.DELETE_FAILURE);
                return true;
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.EXCEPTION)
                    .WithMessage(e.ResultString);
            }
        }

        public async Task<bool> Deleted(IdFromBodyModel fromBodyModel)
        {
            try
            {
                dynamic data = await _context.Set<T>()
                   .Where(x => EF.Property<bool>(x, "IsDeleted") == false && EF.Property<int>(x, "Id") == fromBodyModel.Id) // Lọc theo IsDeleted và Id
                   .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên (hoặc null nếu không có)



                if (data == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);



                var propertyInfo = data.GetType().GetProperty("IsDeleted");
                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(data, true);
                }

                int result = await _context.SaveChangesAsync();

                if (result < 0)
                    throw new ResponseMessageException().WithException(DefaultCode.DELETE_FAILURE);
                return true;
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.EXCEPTION)
                    .WithMessage(e.ResultString);
            }
        }

        public async Task<dynamic> GetAll()
        {
            try
            {

                // Lấy tất cả các bản ghi không bị xóa (IsDeleted = false) và sắp xếp theo trường CreatedAt giảm dần
                var data = await _context.Set<T>()
                    .Where(x => EF.Property<bool>(x, "IsDeleted") == false) // Lọc theo "IsDeleted"
                    .OrderByDescending(x => EF.Property<int>(x, "Id")) // Sắp xếp theo ID từ lớn đến nhỏ
                    .ToListAsync(); // Lấy dữ liệu dưới dạng List

                //var data = await _context.Customers.ToListAsync();

                return data;
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.EXCEPTION)
                    .WithMessage(e.ResultString);
            }

        }

        public async Task<dynamic> GetAllPagingCore(PagingParamDefault pagingParam)
        {
            try
            {
                PagingModel<dynamic> result = new PagingModel<dynamic>();
                IQueryable<T> query = _context.Set<T>();

                // Lọc theo IsDeleted
                query = query.Where(x => EF.Property<bool>(x, "IsDeleted") == false);



                result.TotalRows = await query.CountAsync();
                // Áp dụng phân trang (Skip và Take)
                query = query.Skip(pagingParam.Skip).Take(pagingParam.Limit);
                // Áp dụng Project (chọn các trường bạn cần, giống như projection trong MongoDB)
                var list = await query.ToListAsync(); // Lấy danh sách kết quả

                result.Data = list.ToList();

                return result;

            }
            catch (ResponseMessageException e)
            {
                new ResultMessageResponse().WithCode(e.ResultCode)
                    .WithMessage(e.ResultString);
            }
            return null;
        }

        public async Task<dynamic> GetAllSelected()
        {
            try
            {

                // Lấy tất cả các bản ghi không bị xóa (IsDeleted = false) và sắp xếp theo trường CreatedAt giảm dần
                var filter = await _context.Set<T>()
                    .Where(x => EF.Property<bool>(x, "IsDeleted") == false) // Lọc theo "IsDeleted"
                    .OrderByDescending(x => EF.Property<int>(x, "Id")) // Sắp xếp theo ID từ lớn đến nhỏ
                    .ToListAsync(); // Lấy dữ liệu dưới dạng List

                var data = filter.Select(x => new { id = EF.Property<int>(x, "Id"), name = EF.Property<string>(x, "Name") }) // Chọn các trường Id và Name
            .ToList(); // Lấy dữ liệu dưới dạng List


                return data;
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.EXCEPTION)
                    .WithMessage(e.ResultString);
            }
        }

        public Task<dynamic> GetAllSelectedByCode(string code, string properties)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetById(IdFromBodyModel fromBodyModel)
        {
            try
            {
                var data = await _context.Set<T>()
           .Where(x => EF.Property<bool>(x, "IsDeleted") == false && EF.Property<int>(x, "Id") == fromBodyModel.Id) // Lọc theo IsDeleted và Id
           .FirstOrDefaultAsync(); // Lấy bản ghi đầu tiên (hoặc null nếu không có)


                if (data == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

                return data;
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.EXCEPTION)
                    .WithMessage(e.ResultString);
            }

        }

        public Task<dynamic> GetFirtstDataByProperties(IdFromBodyModel fromBodyModel, string properties)
        {
            throw new NotImplementedException();
        }

        public Task<dynamic> GetListDataByProperties(IdFromBodyModel fromBodyModel, string properties)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetPagingCore(PagingParamDefault pagingParam)
        {
            try
            {
                PagingModel<dynamic> result = new PagingModel<dynamic>();
                IQueryable<T> query = _context.Set<T>();

                // Lọc theo IsDeleted
                query = query.Where(x => EF.Property<bool>(x, "IsDeleted") == false);



                result.TotalRows = await query.CountAsync();
                // Áp dụng phân trang (Skip và Take)
                query = query.Skip(pagingParam.Skip).Take(pagingParam.Limit);
                // Áp dụng Project (chọn các trường bạn cần, giống như projection trong MongoDB)
                var list = await query.ToListAsync(); // Lấy danh sách kết quả

                result.Data = list.ToList();

                return result;

            }
            catch (ResponseMessageException e)
            {
                new ResultMessageResponse().WithCode(e.ResultCode)
                    .WithMessage(e.ResultString);
            }
            return null;
        }
    }
}
