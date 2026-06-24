using badmintion.Interface;
using badmintion.DTO;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly BadmintionNlContext _context;

        public PromotionService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> Create(Promotion model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new PromotionValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var normalizedCode = model.Code!.Trim().ToUpperInvariant();
                if (await _context.Promotions.AnyAsync(x =>
                    x.Code == normalizedCode))
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("Mã khuyến mãi đã tồn tại.");
                }

                var promotion = new Promotion()
                {
                    DiscountType = model.DiscountType,
                    Descriptions = model.Descriptions,
                    DiscountValue = model.DiscountValue,
                    MinOrderValue = model.MinOrderValue,
                    Code = normalizedCode,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    MaxUsage = model.MaxUsage,
                    UsedCount = 0,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };
                await _context.Promotions.AddAsync(promotion);

                await _context.SaveChangesAsync();
                return promotion;

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

        public async Task<dynamic> Update(Promotion model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new PromotionValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.Promotions.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                var normalizedCode = model.Code!.Trim().ToUpperInvariant();
                if (await _context.Promotions.AnyAsync(x =>
                    x.Id != model.Id &&
                    x.Code == normalizedCode))
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("Mã khuyến mãi đã tồn tại.");
                }

                existingCustomer.Descriptions = model.Descriptions;
                existingCustomer.DiscountValue = model.DiscountValue;
                existingCustomer.DiscountType = model.DiscountType;
                existingCustomer.MinOrderValue = model.MinOrderValue;
                existingCustomer.Code = normalizedCode;
                existingCustomer.StartDate = model.StartDate;
                existingCustomer.EndDate = model.EndDate;
                existingCustomer.MaxUsage = model.MaxUsage;
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

        public async Task<dynamic> GetPaging(PromotionPagingRequest model)
        {
            var now = DateTime.Now;
            var query = _context.Promotions.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var search = model.Search.Trim();
                query = query.Where(x =>
                    (x.Code != null && x.Code.Contains(search)) ||
                    (x.Descriptions != null && x.Descriptions.Contains(search)));
            }

            query = model.State?.Trim().ToLowerInvariant() switch
            {
                "active" => query.Where(x =>
                    x.IsDeleted == false &&
                    (!x.StartDate.HasValue || x.StartDate <= now) &&
                    (!x.EndDate.HasValue || x.EndDate >= now) &&
                    (!x.MaxUsage.HasValue || (x.UsedCount ?? 0) < x.MaxUsage)),
                "upcoming" => query.Where(x =>
                    x.IsDeleted == false &&
                    x.StartDate.HasValue &&
                    x.StartDate > now),
                "expired" => query.Where(x =>
                    x.IsDeleted == false &&
                    ((x.EndDate.HasValue && x.EndDate < now) ||
                     (x.MaxUsage.HasValue && (x.UsedCount ?? 0) >= x.MaxUsage))),
                "disabled" => query.Where(x => x.IsDeleted == true),
                _ => query
            };

            var totalRows = await query.CountAsync();
            var data = await query
                .OrderByDescending(x => x.Id)
                .Skip(model.Skip)
                .Take(Math.Max(1, model.Limit))
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Descriptions,
                    x.DiscountType,
                    x.DiscountValue,
                    x.MinOrderValue,
                    x.StartDate,
                    x.EndDate,
                    x.MaxUsage,
                    x.UsedCount,
                    x.IsDeleted,
                    State = x.IsDeleted == true
                        ? "disabled"
                        : x.StartDate.HasValue && x.StartDate > now
                            ? "upcoming"
                            : (x.EndDate.HasValue && x.EndDate < now) ||
                              (x.MaxUsage.HasValue && (x.UsedCount ?? 0) >= x.MaxUsage)
                                ? "expired"
                                : "active"
                })
                .ToListAsync();

            return new PagingModel<dynamic>
            {
                TotalRows = totalRows,
                Data = data
            };
        }

        public async Task<dynamic> SetActive(PromotionStatusRequest model)
        {
            var promotion = await _context.Promotions.FindAsync(model.Id);
            if (promotion == null)
                throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

            promotion.IsDeleted = !model.IsActive;
            await _context.SaveChangesAsync();
            return promotion;
        }

        public async Task<Promotion?> GetValidPromotion(string code, decimal orderValue)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return null;
            }

            var normalizedCode = code.Trim().ToUpperInvariant();
            var now = DateTime.Now;
            return await _context.Promotions.FirstOrDefaultAsync(x =>
                x.IsDeleted == false &&
                x.Code == normalizedCode &&
                (!x.StartDate.HasValue || x.StartDate <= now) &&
                (!x.EndDate.HasValue || x.EndDate >= now) &&
                (!x.MinOrderValue.HasValue || x.MinOrderValue <= orderValue) &&
                (!x.MaxUsage.HasValue || (x.UsedCount ?? 0) < x.MaxUsage));
        }

        public decimal CalculateDiscount(Promotion promotion, decimal orderValue)
        {
            var value = Math.Max(0, promotion.DiscountValue ?? 0);
            var discount = string.Equals(
                promotion.DiscountType,
                "percent",
                StringComparison.OrdinalIgnoreCase)
                ? orderValue * Math.Min(value, 100) / 100
                : value;

            return Math.Min(orderValue, Math.Max(0, discount));
        }
    }
}
