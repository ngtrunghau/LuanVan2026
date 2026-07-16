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
    public class ProductReviewService : IProductReviewService
    {
        private readonly BadmintionNlContext _context;
        private readonly ICurrentUserService _currentUser;

        public ProductReviewService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<dynamic> Create(ProductReviewDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new ProductReviewValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                if (!_currentUser.IsCustomer || !_currentUser.UserId.HasValue)
                    throw new ResponseMessageException().WithException(DefaultCode.NOT_HAVE_ACCESS);

                var canReview = await _context.Orders.AnyAsync(order =>
                    order.Id == model.OrderId &&
                    order.CustomerId == _currentUser.UserId &&
                    order.IsDeleted == false &&
                    order.OrderItems.Any(item =>
                        item.ProductsId == model.ProductId &&
                        item.IsDeleted == false) &&
                    order.ShippingDetails
                        .Where(detail => detail.IsDeleted == false)
                        .OrderByDescending(detail => detail.DateShip)
                        .ThenByDescending(detail => detail.Id)
                        .Select(detail => detail.Status)
                        .FirstOrDefault() == 3);
                if (!canReview)
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.NOT_HAVE_ACCESS)
                        .WithMessage("Chỉ có thể đánh giá sản phẩm thuộc đơn hàng đã hoàn tất.");
                }

                var alreadyReviewed = await _context.ProductReviews.AnyAsync(review =>
                    review.CustomerId == _currentUser.UserId &&
                    review.OrderId == model.OrderId &&
                    review.ProductId == model.ProductId &&
                    review.IsDeleted == false);
                if (alreadyReviewed)
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("Sản phẩm trong đơn hàng này đã được đánh giá.");
                }

                var data = new ProductReview()
                {
                    ProductId = model.ProductId,
                    CustomerId = _currentUser.UserId,
                    OrderId = model.OrderId,
                    UrlImg = model.UrlImg,
                    TotalStar = model.TotalStar,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                    Comment = model.Comment,
                    Date = DateTime.Now,
                    ModerationStatus = 0,
                };


                await _context.ProductReviews.AddAsync(data);

                await _context.SaveChangesAsync();
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

        public async Task<dynamic> Update(ProductReviewDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new ProductReviewValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.ProductReviews.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật
                if (!_currentUser.IsCustomer ||
                    existingCustomer.CustomerId != _currentUser.UserId)
                    throw new ResponseMessageException().WithException(DefaultCode.NOT_HAVE_ACCESS);

                existingCustomer.TotalStar = model.TotalStar;
                existingCustomer.Comment = model.Comment;
                existingCustomer.Date = DateTime.Now;
                existingCustomer.UrlImg = model.UrlImg;
                existingCustomer.ModerationStatus = 0;
                existingCustomer.ModerationReason = null;
                existingCustomer.ModeratedAt = null;
                existingCustomer.ModeratedBy = null;
                var saveResult = await _context.SaveChangesAsync();

                

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

        public async Task<dynamic> GetModerationList(ProductReviewFilterDTO model)
        {
            var page = Math.Max(1, model.Start);
            var limit = Math.Clamp(model.Limit, 1, 100);
            var keyword = model.Keyword?.Trim();

            var query = _context.ProductReviews
                .AsNoTracking()
                .Where(x => x.IsDeleted == false);

            if (model.Status.HasValue)
                query = query.Where(x => x.ModerationStatus == model.Status.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x =>
                    (x.Comment != null && x.Comment.Contains(keyword)) ||
                    (x.Product != null && x.Product.Name.Contains(keyword)));

            var totalRows = await query.LongCountAsync();
            var data = await query
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new
                {
                    x.Id,
                    x.ProductId,
                    ProductName = x.Product != null ? x.Product.Name : null,
                    ProductImage = x.Product != null ? x.Product.ImageUrl : null,
                    x.CustomerId,
                    CustomerName = _context.Customers
                        .Where(customer => customer.Id == x.CustomerId)
                        .Select(customer => customer.FullName)
                        .FirstOrDefault(),
                    x.OrderId,
                    x.TotalStar,
                    x.Comment,
                    x.UrlImg,
                    x.Date,
                    x.ModerationStatus,
                    x.ModerationReason,
                    x.ModeratedAt,
                    x.ModeratedBy
                })
                .ToListAsync();

            return new { TotalRows = totalRows, Data = data };
        }

        public async Task<dynamic> Moderate(ProductReviewModerationDTO model)
        {
            if (model.Status is not (1 or 2))
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Trạng thái kiểm duyệt không hợp lệ.");

            if (model.Status == 2 && string.IsNullOrWhiteSpace(model.Reason))
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Vui lòng nhập lý do ẩn đánh giá.");

            var review = await _context.ProductReviews
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.IsDeleted == false);
            if (review == null)
                throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

            review.ModerationStatus = model.Status;
            review.ModerationReason = model.Status == 2 ? model.Reason?.Trim() : null;
            review.ModeratedAt = DateTime.Now;
            review.ModeratedBy = _currentUser.UserName ?? _currentUser.UserId?.ToString();
            await _context.SaveChangesAsync();

            return new
            {
                review.Id,
                review.ModerationStatus,
                review.ModerationReason,
                review.ModeratedAt,
                review.ModeratedBy
            };
        }

        public async Task<dynamic> GetApprovedByProduct(int productId)
        {
            return await _context.ProductReviews
                .AsNoTracking()
                .Where(x =>
                    x.ProductId == productId &&
                    x.IsDeleted == false &&
                    x.ModerationStatus == 1)
                .OrderByDescending(x => x.Date)
                .Select(x => new
                {
                    x.Id,
                    x.TotalStar,
                    x.Comment,
                    x.UrlImg,
                    x.Date,
                    CustomerName = _context.Customers
                        .Where(customer => customer.Id == x.CustomerId)
                        .Select(customer => customer.FullName)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }
    }
}
