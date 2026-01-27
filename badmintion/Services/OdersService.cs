using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Others;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class OdersService : IOdersService
    {
        private readonly BadmintionNlContext _context;
        private readonly IVnPayService _vnPay;
        public OdersService(BadmintionNlContext context, IHttpContextAccessor contextAccessor, IVnPayService vnPay)
        {
            _context = context;
            _vnPay = vnPay;
        }

        public async Task<dynamic> Create(OrdersDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new OdersValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var order = new Order()
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = model.TotalAmount,
                    Status = 1,
                    CustomerId = model.CustomerId == null ? null : model.CustomerId,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                    AddressId = model.AddressId
                };
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                foreach (var item in model.ListOrderItems)
                {
                    var orderItem = new OrderItem()
                    {
                        Quantity = item.Quantity,
                        Price = item.Price,
                        ProductsId = item.ProductsId,
                        Orders = order.Id,
                        IsDeleted = item.IsDeleted == null ? false : item.IsDeleted,
                        
                    };
                    await _context.OrderItems.AddAsync(orderItem);
                    await _context.SaveChangesAsync();

                }
                var ship = new ShippingDetail()
                {
                    IsDeleted = false,
                    Status = 1,
                    OrdersId = order.Id,
                    DateShip = DateTime.Now,
                };
                await _context.ShippingDetails.AddAsync(ship);
                await _context.SaveChangesAsync();
                var money = Convert.ToDouble(model.TotalAmount);
                var payment = await _vnPay.CreatePaymentUrl(money, order.Id);

                var response = new ResponsePayment()
                {
                    Url = payment,
                    OrderId = order.Id,
                };
                return response;

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
        public async Task<dynamic> GetPagingCore(PagingParamDefault pagingParam)
        {
            try
            {

                PagingModel<dynamic> result = new PagingModel<dynamic>();
                var data = await _context.Orders.Where(x => x.IsDeleted == false && x.ShippingDetails.Any(x => x.Status == 1)).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x=>x.Customer).Include(x=>x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    o.Address,
                    o.Customer,
                    o.ShippingDetails,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.Id,
                        i.Products,
                        i.Quantity,
                        i.Price
                    }).ToList(),
                }).ToListAsync();

                result.Data = data;
                result.TotalRows = data.Count();
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
        public async Task<dynamic> GetPagingCoreStatus2(PagingParamDefault pagingParam)
        {
            try
            {

                PagingModel<dynamic> result = new PagingModel<dynamic>();
                var data = await _context.Orders.Where(x => x.IsDeleted == false && x.ShippingDetails.Any(x => x.Status == 2)).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    o.Address,
                    o.Customer,
                    o.ShippingDetails,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.Id,
                        i.Products,
                        i.Quantity,
                        i.Price
                    }).ToList(),
                }).ToListAsync();

                result.Data = data;
                result.TotalRows = data.Count();
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
        public async Task<dynamic> GetPagingCoreStatus3(PagingParamDefault pagingParam)
        {
            try
            {

                PagingModel<dynamic> result = new PagingModel<dynamic>();
                var data = await _context.Orders.Where(x => x.IsDeleted == false && x.ShippingDetails.Any(x => x.Status == 3 )).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    o.Address,
                    o.Customer,
                    o.ShippingDetails,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.Id,
                        i.Products,
                        i.Quantity,
                        i.Price
                    }).ToList(),
                }).ToListAsync();

                result.Data = data;
                result.TotalRows = data.Count();
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
        public async Task<dynamic> GetIdCore(int id)
        {
            try
            {

     
                var data = await _context.Orders.Where(x => x.IsDeleted == false && x.Id == id).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    o.Address,
                    o.Customer,
                    o.TxnRef,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.Id,
                        i.Products,
                        i.Quantity,
                        i.Price
                    }).ToList(),
                    ShippingDetail = o.ShippingDetails.Where(x => x.Status == 1).ToList(),
                }).FirstOrDefaultAsync();

           
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
        public async Task<dynamic> Update(OrdersDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new OdersValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.Orders.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

             
                existingCustomer.Status = model.Status;
                
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

        public async Task<dynamic> GetPagingCoreStatus3Customer(PagingParamDefault pagingParam)
        {
            try
            {

                PagingModel<dynamic> result = new PagingModel<dynamic>();
                var data = await _context.Orders.Where(x => x.IsDeleted == false && x.Customer.Id == pagingParam.IdDonViCha && x.ShippingDetails.Any(x => x.Status == 3)).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    o.Address,
                    o.Customer,
                    o.ShippingDetails,
                    Items = o.OrderItems.Select(i => new
                    {
                        i.Id,
                        i.Products,
                        i.Quantity,
                        i.Price
                    }).ToList(),
                }).ToListAsync();

                result.Data = data;
                result.TotalRows = data.Count();
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
