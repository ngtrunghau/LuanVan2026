using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class DistrictService : IDistrictService
    {
        private readonly BadmintionNlContext _context;

        public DistrictService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> GetAllByIdProvince(int id)
        {
            try
            {

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.Districts.Where(x => x.ProvinceId == id).ToListAsync();

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
