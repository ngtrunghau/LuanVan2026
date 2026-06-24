using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace badmintion.Services
{
    public class ProductService : IProductService
    {
        private readonly BadmintionNlContext _context;

        public ProductService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> Create(ProductDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new ProductValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var product = new Product()
                {
                    Name = model.Name,
                    Descriptions = model.Descriptions,
                    Price = model.Price,
                    StockQuantity = 0,
                    ImageUrl = model.ImageUrl,
                    Color = model.Color,
                    CategoriesId = model.CategoriesId,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };
                await _context.Products.AddAsync(product);

                await _context.SaveChangesAsync();
                return product;

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

        public async Task<dynamic> GetByIdCate(PagingParam pagingParam)
        {
            try
            {

                PagingModel<dynamic> result = new PagingModel<dynamic>();
                var query = _context.Products
                    .Where(x => x.IsDeleted == false && x.CategoriesId == pagingParam.IdDonViCha);

                if (pagingParam.MinPrice.HasValue)
                {
                    query = query.Where(x => x.Price >= pagingParam.MinPrice.Value);
                }

                if (pagingParam.MaxPrice.HasValue)
                {
                    query = query.Where(x => x.Price <= pagingParam.MaxPrice.Value);
                }

                result.TotalRows = await query.CountAsync();
                result.Data = await query.Skip(pagingParam.Skip).Take(pagingParam.Limit).ToListAsync();
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

        public async Task<dynamic> Update(ProductDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new ProductValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.Products.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                existingCustomer.Name = model.Name;
                existingCustomer.Descriptions = model.Descriptions;
                existingCustomer.Price = model.Price;
                //existingCustomer.StockQuantity = model.StockQuantity;
                existingCustomer.ImageUrl = model.ImageUrl;
                existingCustomer.Color = model.Color;
                existingCustomer.CategoriesId = model.CategoriesId;
                existingCustomer.IsDeleted = model.IsDeleted;

                var saveResult = await _context.SaveChangesAsync();

                // Kiểm tra xem có thay đổi nào được lưu vào cơ sở dữ liệu không
                //if (saveResult <= 0)
                //    throw new ResponseMessageException().WithException(DefaultCode.UPDATE_FAILURE);

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
