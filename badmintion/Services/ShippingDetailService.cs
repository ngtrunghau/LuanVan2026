using badmintion.DTO;
using badmintion.Interface;
using badmintion.Contansts;
using badmintion.Interface.Core;
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
        private readonly ICurrentUserService _currentUser;

        public ShippingDetailService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
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
                

                var customerId = _currentUser.IsCustomer ? _currentUser.UserId : id;
                if (!customerId.HasValue)
                    throw new ResponseMessageException().WithException(DefaultCode.NOT_HAVE_ACCESS);

                var existingCustomer = await _context.Orders
                    .Where(x => x.CustomerId == customerId.Value && x.IsDeleted == false)
                    .OrderByDescending(x => x.OrderDate)
                    .Select(order => new
                    {
                        order.Id,
                        order.OrderDate,
                        order.TotalAmount,
                        order.TxnRef,
                        Status = order.ShippingDetails
                            .Where(x => x.IsDeleted == false)
                            .OrderByDescending(x => x.DateShip)
                            .ThenByDescending(x => x.Id)
                            .Select(x => x.Status)
                            .FirstOrDefault(),
                        order.Customer,
                        order.Address,
                        OrderItems = order.OrderItems
                            .Where(x => x.IsDeleted == false)
                            .Select(x => new
                            {
                                x.Id,
                                x.Products,
                                x.Quantity,
                                x.Price
                            })
                            .ToList(),
                        ShippingDetails = order.ShippingDetails
                            .Where(x => x.IsDeleted == false)
                            .OrderByDescending(x => x.DateShip)
                            .ThenByDescending(x => x.Id)
                            .Select(x => new
                            {
                                x.Id,
                                x.Status,
                                x.DateShip,
                                x.OrdersId,
                                x.IsDeleted,
                                x.Note,
                                x.ChangedBy,
                                x.ChangedByType
                            })
                            .ToList()
                    })
                    .ToListAsync();
                

             
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

               
                var existingCustomer = await _context.ShippingDetails
                    .Where(x =>
                        x.OrdersId == id &&
                        x.IsDeleted == false &&
                        (!_currentUser.IsCustomer || x.Orders!.CustomerId == _currentUser.UserId))
                    .Include(x => x.Orders)
                    .OrderByDescending(x => x.DateShip)
                    .ThenByDescending(x => x.Id)
                    .ToListAsync();
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
                var order = await _context.Orders
                    .Include(x => x.Payments)
                    .Include(x => x.OrderItems)
                        .ThenInclude(x => x.Products)
                            .ThenInclude(x => x.WareHouses)
                    .Include(x => x.ShippingDetails)
                    .FirstOrDefaultAsync(x => x.Id == model.OrdersId && x.IsDeleted == false);
                if (order == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

                var latestShippingDetail = order.ShippingDetails
                    .Where(x => x.IsDeleted == false)
                    .OrderByDescending(x => x.DateShip)
                    .ThenByDescending(x => x.Id)
                    .FirstOrDefault();
                if (latestShippingDetail == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

                var nextStatus = model.Status ?? 0;
                var currentStatus = latestShippingDetail.Status ?? 0;
                if (nextStatus == currentStatus)
                {
                    return latestShippingDetail;
                }
                var isCancellation = nextStatus == OrderStatus.Cancelled;
                var isNormalTransition =
                    nextStatus == currentStatus + 1 &&
                    nextStatus is >= OrderStatus.Pending and <= OrderStatus.Completed;
                var canCancel =
                    isCancellation &&
                    currentStatus is OrderStatus.Pending or OrderStatus.Shipping &&
                    (!_currentUser.IsCustomer || currentStatus == OrderStatus.Pending);
                if (!isNormalTransition && !canCancel)
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("Chuyển trạng thái giao hàng không hợp lệ.");
                }

                if (_currentUser.IsCustomer && order.CustomerId != _currentUser.UserId)
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.NOT_HAVE_ACCESS)
                        .WithMessage("Bạn không có quyền cập nhật đơn hàng này.");
                }

                if (nextStatus == OrderStatus.Shipping && order.Status != OrderStatus.Shipping)
                {
                    DeductInventory(order);
                }

                if (nextStatus == OrderStatus.Completed)
                {
                    var payment = order.Payments
                        .Where(x => x.IsDeleted == false)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                    if (payment != null && string.Equals(payment.PaymentMethod, "cod", StringComparison.OrdinalIgnoreCase))
                    {
                        payment.PaymentStatus = "paid";
                    }
                }
                else if (nextStatus == OrderStatus.Cancelled)
                {
                    if (currentStatus == OrderStatus.Shipping)
                    {
                        RestoreInventory(order);
                    }

                    var payment = order.Payments
                        .Where(x => x.IsDeleted == false)
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault();
                    if (payment != null)
                    {
                        payment.PaymentStatus = "cancelled";
                    }
                }

                order.Status = nextStatus;
                var newLog = new ShippingDetail
                {
                    DateShip = DateTime.Now,
                    Status = nextStatus,
                    OrdersId = order.Id,
                    IsDeleted = false,
                    Note = string.IsNullOrWhiteSpace(model.Note)
                        ? GetDefaultStatusNote(nextStatus)
                        : model.Note.Trim(),
                    ChangedBy = _currentUser.UserId?.ToString(),
                    ChangedByType = _currentUser.IsCustomer ? "customer" : "admin"
                };
                await _context.ShippingDetails.AddAsync(newLog);
                var saveResult = await _context.SaveChangesAsync();

                if (saveResult <= 0)
                    throw new ResponseMessageException().WithException(DefaultCode.UPDATE_FAILURE);

                return newLog;
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

        private static void DeductInventory(Order order)
        {
            foreach (var orderItem in order.OrderItems.Where(x => x.IsDeleted == false))
            {
                var product = orderItem.Products;
                var quantityToDeduct = Math.Max(0, orderItem.Quantity ?? 0);
                if (product == null || quantityToDeduct == 0)
                {
                    continue;
                }

                if ((product.StockQuantity ?? 0) < quantityToDeduct)
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.EXCEPTION)
                        .WithMessage($"Sản phẩm '{product.Name}' không đủ tồn kho.");
                }

                product.StockQuantity = Math.Max(0, (product.StockQuantity ?? 0) - quantityToDeduct);
                var remaining = quantityToDeduct;
                foreach (var warehouse in product.WareHouses
                             .Where(x => x.IsDeleted == false && (x.RemainQuantity ?? 0) > 0)
                             .OrderBy(x => x.Id))
                {
                    if (remaining <= 0)
                    {
                        break;
                    }

                    var available = warehouse.RemainQuantity ?? 0;
                    var deducted = Math.Min(available, remaining);
                    warehouse.RemainQuantity = available - deducted;
                    remaining -= deducted;
                }
            }
        }

        private static void RestoreInventory(Order order)
        {
            foreach (var orderItem in order.OrderItems.Where(x => x.IsDeleted == false))
            {
                var product = orderItem.Products;
                var quantityToRestore = Math.Max(0, orderItem.Quantity ?? 0);
                if (product == null || quantityToRestore == 0)
                {
                    continue;
                }

                product.StockQuantity = (product.StockQuantity ?? 0) + quantityToRestore;
                var warehouse = product.WareHouses
                    .Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.Id)
                    .FirstOrDefault();
                if (warehouse != null)
                {
                    warehouse.RemainQuantity = (warehouse.RemainQuantity ?? 0) + quantityToRestore;
                }
            }
        }

        private static string GetDefaultStatusNote(int status)
        {
            return status switch
            {
                OrderStatus.Pending => "Đơn hàng đang chờ xác nhận.",
                OrderStatus.Shipping => "Đơn hàng đã được xác nhận và chuyển sang vận chuyển.",
                OrderStatus.Completed => "Đơn hàng đã được giao thành công.",
                OrderStatus.Cancelled => "Đơn hàng đã bị hủy.",
                _ => "Trạng thái đơn hàng đã được cập nhật."
            };
        }
    }
}
