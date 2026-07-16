using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace badmintion.Services
{
    public class AddressCustomerService : IAddressCustomerService
    {
        private readonly BadmintionNlContext _context;
        private readonly ICurrentUserService _currentUser;

        public AddressCustomerService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<dynamic> Create(AddressCustomerDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new AddressCustomerValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var address = new AddressCustomer()
                {
                    ProvinceId = model.ProvinceId,
                    DistrictId = model.DistrictId,
                    TownId = model.TownId,
                    Address = model.Address,
                    CustomerId = _currentUser.IsCustomer
                        ? _currentUser.UserId
                        : model.CustomerId,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };


                await _context.AddressCustomers.AddAsync(address);

                await _context.SaveChangesAsync();
                return address;

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

        public async Task<dynamic> GetAllByIdUser(int id)
        {
            try
            {
                if (id == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);
                var customerId = _currentUser.IsCustomer ? _currentUser.UserId : id;
                if (!customerId.HasValue)
                    throw new ResponseMessageException().WithException(DefaultCode.NOT_HAVE_ACCESS);

                var data = await _context.AddressCustomers
                    .Where(x => x.IsDeleted == false && x.CustomerId == customerId.Value)
                    .ToListAsync();
                if (data == null)
                {
                    return null;
                }
                var listAdd = new List<AddressDetail>();
                foreach (var item in data)
                {
                    
                    var provi = await _context.Provinces.FindAsync(item.ProvinceId);
                    var dis = await _context.Districts.FindAsync(item.DistrictId);
                    var town = await _context.Towns.FindAsync(item.TownId);
                    var uAdd = new AddressDetail() { 
                        Province = provi.Name,
                        District = dis.Name,
                        Town = town.Name,
                        CustomerId = item.CustomerId,
                        Address = item.Address,
                        Id = item.Id,
                    };
                    listAdd.Add(uAdd);
                }
                return listAdd;
               

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

        public async Task<dynamic> Update(AddressCustomerDTO model)
        {
            
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new AddressCustomerValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.AddressCustomers.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật
                if (_currentUser.IsCustomer && existingCustomer.CustomerId != _currentUser.UserId)
                    throw new ResponseMessageException().WithException(DefaultCode.NOT_HAVE_ACCESS);

                existingCustomer.ProvinceId = model.ProvinceId;
                existingCustomer.DistrictId = model.DistrictId;
                existingCustomer.TownId = model.TownId;
                existingCustomer.Address = model.Address;
                existingCustomer.IsDeleted = model.IsDeleted;
                var saveResult = await _context.SaveChangesAsync();

                // Kiểm tra xem có thay đổi nào được lưu vào cơ sở dữ liệu không
                if (saveResult <= 0)
                    throw new ResponseMessageException().WithException(DefaultCode.UPDATE_FAILURE);

                // Trả về đối tượng đã cập nhật (hoặc có thể trả về thông tin khác nếu cần)
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
