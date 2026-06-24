using badmintion.DTO;
using badmintion.Contansts;
using badmintion.Interface;
using badmintion.Interface.Core;
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
        private readonly ICurrentUserService _currentUser;
        private readonly IPromotionService _promotionService;
        public OdersService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            ICurrentUserService currentUser,
            IPromotionService promotionService)
        {
            _context = context;
            _currentUser = currentUser;
            _promotionService = promotionService;
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

                var requestedItems = model.ListOrderItems
                    .GroupBy(x => x.ProductsId!.Value)
                    .Select(x => new
                    {
                        ProductId = x.Key,
                        Quantity = x.Sum(i => i.Quantity ?? 0)
                    })
                    .ToList();
                var productIds = requestedItems.Select(x => x.ProductId).ToList();
                var products = await _context.Products
                    .Where(x => productIds.Contains(x.Id) && x.IsDeleted == false)
                    .ToDictionaryAsync(x => x.Id);

                if (products.Count != productIds.Count)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

                foreach (var requested in requestedItems)
                {
                    var product = products[requested.ProductId];
                    if (requested.Quantity <= 0 || (product.StockQuantity ?? 0) < requested.Quantity)
                    {
                        throw new ResponseMessageException()
                            .WithCode(DefaultCode.EXCEPTION)
                            .WithMessage($"Sản phẩm '{product.Name}' không đủ tồn kho.");
                    }
                }

                const decimal shippingFee = 30_000m;
                var calculatedSubtotal = requestedItems.Sum(x =>
                    (products[x.ProductId].Price ?? 0m) * x.Quantity);
                Promotion? promotion = null;
                var discountAmount = 0m;
                if (!string.IsNullOrWhiteSpace(model.PromotionCode))
                {
                    promotion = await _promotionService.GetValidPromotion(
                        model.PromotionCode,
                        calculatedSubtotal);
                    if (promotion == null)
                    {
                        throw new ResponseMessageException()
                            .WithCode(DefaultCode.ERROR_STRUCTURE)
                            .WithMessage("Mã khuyến mãi không hợp lệ hoặc đã hết hạn.");
                    }

                    discountAmount = _promotionService.CalculateDiscount(
                        promotion,
                        calculatedSubtotal);
                }

                var calculatedTotal = calculatedSubtotal - discountAmount + shippingFee;
                var paymentMethod = (model.PaymentMethod ?? "cod").Trim().ToLowerInvariant();
                if (paymentMethod is not ("cod" or "bank"))
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("Phương thức thanh toán không hợp lệ.");
                }

                var customerId = _currentUser.IsCustomer
                    ? _currentUser.UserId
                    : model.CustomerId;
                if (!customerId.HasValue)
                    throw new ResponseMessageException().WithException(DefaultCode.NOT_HAVE_ACCESS);

                var addressBelongsToCustomer = await _context.AddressCustomers.AnyAsync(x =>
                    x.Id == model.AddressId &&
                    x.CustomerId == customerId.Value &&
                    x.IsDeleted == false);
                if (!addressBelongsToCustomer)
                    throw new ResponseMessageException().WithException(DefaultCode.NOT_HAVE_ACCESS);

                await using var transaction = await _context.Database.BeginTransactionAsync();
                var order = new Order()
                {
                    OrderDate = DateTime.Now,
                    TotalAmount = calculatedTotal,
                    Status = OrderStatus.Pending,
                    CustomerId = customerId,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                    AddressId = model.AddressId,
                    PromotionId = promotion?.Id,
                    DiscountAmount = discountAmount
                };
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
                foreach (var item in requestedItems)
                {
                    var orderItem = new OrderItem()
                    {
                        Quantity = item.Quantity,
                        Price = products[item.ProductId].Price ?? 0m,
                        ProductsId = item.ProductId,
                        Orders = order.Id,
                        IsDeleted = false,
                        
                    };
                    await _context.OrderItems.AddAsync(orderItem);
                }
                await _context.SaveChangesAsync();
                var ship = new ShippingDetail()
                {
                    IsDeleted = false,
                    Status = OrderStatus.Pending,
                    OrdersId = order.Id,
                    DateShip = DateTime.Now,
                    Note = "Đơn hàng đã được tạo.",
                    ChangedBy = customerId.Value.ToString(),
                    ChangedByType = _currentUser.IsCustomer ? "customer" : "admin",
                };
                await _context.ShippingDetails.AddAsync(ship);
                await _context.SaveChangesAsync();
                var paymentRecord = new Payment
                {
                    OrdersId = order.Id,
                    PaymentMethod = paymentMethod,
                    PaymentStatus = "pending",
                    IsDeleted = false
                };
                await _context.Payments.AddAsync(paymentRecord);
                if (promotion != null)
                {
                    promotion.UsedCount = (promotion.UsedCount ?? 0) + 1;
                }
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ResponsePayment
                {
                    Url = string.Empty,
                    OrderId = order.Id,
                };

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
                var data = await _context.Orders.Where(x =>
                    x.IsDeleted == false &&
                    (!_currentUser.IsCustomer || x.CustomerId == _currentUser.UserId) &&
                    x.ShippingDetails
                    .Where(s => s.IsDeleted == false)
                    .OrderByDescending(s => s.DateShip)
                    .ThenByDescending(s => s.Id)
                    .Select(s => s.Status)
                    .FirstOrDefault() == 1).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x=>x.Customer).Include(x=>x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    Status = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .Select(s => s.Status)
                        .FirstOrDefault(),
                    o.Address,
                    o.Customer,
                    ShippingDetails = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .ToList(),
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
                var data = await _context.Orders.Where(x =>
                    x.IsDeleted == false &&
                    (!_currentUser.IsCustomer || x.CustomerId == _currentUser.UserId) &&
                    x.ShippingDetails
                    .Where(s => s.IsDeleted == false)
                    .OrderByDescending(s => s.DateShip)
                    .ThenByDescending(s => s.Id)
                    .Select(s => s.Status)
                    .FirstOrDefault() == 2).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    Status = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .Select(s => s.Status)
                        .FirstOrDefault(),
                    o.Address,
                    o.Customer,
                    ShippingDetails = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .ToList(),
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
                var data = await _context.Orders.Where(x =>
                    x.IsDeleted == false &&
                    (!_currentUser.IsCustomer || x.CustomerId == _currentUser.UserId) &&
                    x.ShippingDetails
                    .Where(s => s.IsDeleted == false)
                    .OrderByDescending(s => s.DateShip)
                    .ThenByDescending(s => s.Id)
                    .Select(s => s.Status)
                    .FirstOrDefault() == 3).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    Status = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .Select(s => s.Status)
                        .FirstOrDefault(),
                    o.Address,
                    o.Customer,
                    ShippingDetails = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .ToList(),
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

     
                var data = await _context.Orders.Where(x =>
                    x.IsDeleted == false &&
                    x.Id == id &&
                    (!_currentUser.IsCustomer || x.CustomerId == _currentUser.UserId)).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.DiscountAmount,
                    o.OrderDate,
                    o.Address,
                    o.Customer,
                    o.TxnRef,
                    Payments = o.Payments
                        .Where(x => x.IsDeleted == false)
                        .OrderByDescending(x => x.Id)
                        .Select(x => new
                        {
                            x.PaymentMethod,
                            x.PaymentStatus,
                            x.BillId
                        })
                        .ToList(),
                    Status = o.ShippingDetails
                        .Where(x => x.IsDeleted == false)
                        .OrderByDescending(x => x.DateShip)
                        .ThenByDescending(x => x.Id)
                        .Select(x => x.Status)
                        .FirstOrDefault(),
                    Items = o.OrderItems.Select(i => new
                    {
                        i.Id,
                        i.Products,
                        i.Quantity,
                        i.Price
                    }).ToList(),
                    ShippingDetail = o.ShippingDetails
                        .Where(x => x.IsDeleted == false)
                        .OrderByDescending(x => x.DateShip)
                        .ThenByDescending(x => x.Id)
                        .Take(1)
                        .ToList(),
                    OrderLogs = o.ShippingDetails
                        .Where(x => x.IsDeleted == false)
                        .OrderByDescending(x => x.DateShip)
                        .ThenByDescending(x => x.Id)
                        .ToList(),
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
                var requestedCustomerId = _currentUser.IsCustomer
                    ? _currentUser.UserId
                    : pagingParam.IdDonViCha;
                var data = await _context.Orders.Where(x => x.IsDeleted == false && x.Customer.Id == requestedCustomerId && x.ShippingDetails
                    .Where(s => s.IsDeleted == false)
                    .OrderByDescending(s => s.DateShip)
                    .ThenByDescending(s => s.Id)
                    .Select(s => s.Status)
                    .FirstOrDefault() == 3).Include(x => x.ShippingDetails).Include(x => x.Address).ThenInclude(x => x.Town).ThenInclude(x => x.District).ThenInclude(x => x.Province).OrderByDescending(x => x.OrderDate).Include(x => x.Customer).Include(x => x.OrderItems).Skip(pagingParam.Skip).Take(pagingParam.Limit).Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.OrderDate,
                    Status = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .Select(s => s.Status)
                        .FirstOrDefault(),
                    o.Address,
                    o.Customer,
                    ShippingDetails = o.ShippingDetails
                        .Where(s => s.IsDeleted == false)
                        .OrderByDescending(s => s.DateShip)
                        .ThenByDescending(s => s.Id)
                        .ToList(),
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
