using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Text;

namespace badmintion.Services
{
    public class CustomerService :  ICustomerService
    {
        private readonly BadmintionNlContext _context;
        private readonly IPasswordService _passwordService;
        private readonly ICurrentUserService _currentUser;
        private readonly JwtSettings _jwtSettings;

        public CustomerService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            IPasswordService passwordService,
            IOptions<JwtSettings> jwtSettings,
            ICurrentUserService currentUser)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtSettings = jwtSettings.Value;
            _currentUser = currentUser;
        }
        
        public async Task<dynamic> Create(CustomersDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new CustomerValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);


                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.UserName == model.UserName && x.IsDeleted == false);
                if (existingCustomer != null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_EXISTED);  // Nếu không tìm thấy đối tượng cần cập nhật

                var customer = new Customer()
                {
                    FullName = model.FullName,
                    Phone = model.Phone,
                    Email = model.Email,
                    UserName = model.UserName,
                    Password = _passwordService.Hash(model.Password),
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };
               
                await _context.Customers.AddAsync(customer);

                await _context.SaveChangesAsync();
                return ToCustomerResponse(customer);

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

        public async Task<dynamic> Login(CustomersDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new CustomerValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(
                    x => x.IsDeleted == false && x.UserName == model.UserName);
                if (existingCustomer == null ||
                    !_passwordService.Verify(model.Password, existingCustomer.Password))
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                if (_passwordService.NeedsRehash(existingCustomer.Password))
                {
                    existingCustomer.Password = _passwordService.Hash(model.Password);
                    await _context.SaveChangesAsync();
                }

                return ToCustomerResponse(existingCustomer, CreateCustomerToken(existingCustomer));
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

        public async Task<dynamic> Update(CustomersDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new CustomerValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var customerId = _currentUser.IsCustomer ? _currentUser.UserId : model.Id;
                var existingCustomer = customerId.HasValue
                    ? await _context.Customers.FindAsync(customerId.Value)
                    : null;
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                existingCustomer.FullName = model.FullName;
                existingCustomer.Phone = model.Phone;
                existingCustomer.Email = model.Email;
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
                return ToCustomerResponse(existingCustomer);
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
        private string CreateCustomerToken(Customer customer)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("account_type", "customer"),
                new Claim(
                    JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static object ToCustomerResponse(Customer customer, string? accessToken = null)
        {
            return new
            {
                customer.Id,
                customer.FullName,
                customer.Phone,
                customer.Email,
                customer.UserName,
                customer.IsDeleted,
                AccessToken = accessToken
            };
        }
    }
}
