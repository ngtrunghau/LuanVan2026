using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly BadmintionNlContext _context;

        public ProvinceService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> GetAll()
        {
            try
            {

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.Provinces.ToListAsync();
               
                return existingCustomer;

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
