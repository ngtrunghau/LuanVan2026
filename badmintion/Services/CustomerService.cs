using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Numerics;

namespace badmintion.Services
{
    public class CustomerService :  ICustomerService
    {
        private readonly BadmintionNlContext _context;

        public CustomerService(BadmintionNlContext context, IHttpContextAccessor contextAccessor) 
        {
            _context = context;
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
                    Password = model.Password,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };
               
                await _context.Customers.AddAsync(customer);

                await _context.SaveChangesAsync();
                return model;

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
                var existingCustomer = await _context.Customers.FirstOrDefaultAsync(x => x.IsDeleted == false && x.UserName == model.UserName && x.Password == model.Password);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

               

             
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
                var existingCustomer = await _context.Customers.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                existingCustomer.FullName = model.FullName;
                existingCustomer.Phone = model.Phone;
                existingCustomer.Email = model.Email;
                existingCustomer.UserName = model.UserName;
                existingCustomer.Password = model.Password;
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
