using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace badmintion.Services
{
    public class ShippingDetailService : IShippingDetailService
    {
        private readonly BadmintionNlContext _context;

        public ShippingDetailService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> Create(ShippingDetailDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new ShippingDetailValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var ship = new ShippingDetail()
                {
                    DateShip = model.DateShip == null ? DateTime.Now : model.DateShip,
                    Status = model.Status,
                    OrdersId = model.OrdersId,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };
                await _context.ShippingDetails.AddAsync(ship);

                await _context.SaveChangesAsync();
                return ship;

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
       
        public async Task<dynamic> GetByIdCustomer(int id)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (id == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);
                

                var existingCustomer = await _context.Orders.Where(x => x.CustomerId == id && x.IsDeleted == false).Include(x => x.OrderItems).ThenInclude(oi => oi.Products).Include(x => x.Customer).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).ToListAsync();
                

             
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

        public async Task<dynamic> GetByIdOrder(int id)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (id == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

               
                var existingCustomer = await _context.ShippingDetails.Where(x => x.OrdersId == id && x.IsDeleted == false).Include(x => x.Orders).OrderByDescending(x => x.DateShip).ToListAsync();
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

        public async Task<dynamic> Update(ShippingDetailDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new ShippingDetailValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.ShippingDetails.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                existingCustomer.DateShip = DateTime.Now;
                existingCustomer.Status = model.Status;
                existingCustomer.OrdersId = model.OrdersId;
                existingCustomer.IsDeleted = false;
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
