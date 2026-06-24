using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class UserService : IUserService
    {
        private readonly BadmintionNlContext _context;
        private readonly IPasswordService _passwordService;

        public UserService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<dynamic> Create(UserDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new UserValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var user = new User()
                {
                    Name = model.Name,
                    UserName = model.UserName,
                    Password = _passwordService.Hash(model.Password!),
                    UnitRoleId = model.UnitRoleId,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };
                await _context.Users.AddAsync(user);

                await _context.SaveChangesAsync();
                return user;

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

        public async Task<dynamic> GetByIDCore(int id)
        {
            try
            {
                if (id == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                var user = await  _context.Users.Where(x => x.IsDeleted == false && x.Id == id).Include(o => o.UnitRole).FirstOrDefaultAsync();

                var data = new
                {
                    Id = user.Id,
                    Name = user.Name,
                    Username = user.UserName,
                    IsDelete = user.IsDeleted ,
                    UnitRole = new
                    {
                        Id = user.UnitRole.Id,
                        Name = user.UnitRole.Name,
                    }
                };

                return data;

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

       

        public async Task<dynamic> Update(UserDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new UserValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.Users.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                existingCustomer.Name = model.Name;
                existingCustomer.UnitRoleId = model.UnitRoleId;
                existingCustomer.UserName = model.UserName;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    existingCustomer.Password = _passwordService.Hash(model.Password);
                }
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
