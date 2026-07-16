using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class WareHouseService : IWareHouseService
    {
        private readonly BadmintionNlContext _context;

        public WareHouseService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }
        //api thêm sp mới vào kho
        public async Task<dynamic> Create(WareHouseDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new WareHouseValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật


                var ware = await _context.WareHouses.FirstOrDefaultAsync(x => x.ProductId == model.ProductId);
                WareHouse wareHouse;
                if (ware == null)
                {
                    wareHouse = new WareHouse()
                    {
                        Name = product.Name,
                        IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                        ProductId = model.ProductId,
                        QuantityImport = model.QuantityImport,
                        RemainQuantity = model.QuantityImport,
                    };
                    product.StockQuantity = model.QuantityImport;
                    await _context.WareHouses.AddAsync(wareHouse);
                }
                else
                {
                    wareHouse = ware;
                    ware.QuantityImport = ware.QuantityImport + model.QuantityImport;
                    ware.RemainQuantity = ware.RemainQuantity + model.QuantityImport;

                    product.StockQuantity = ware.RemainQuantity;
                }




                var history = new HistoryImport(){
                    Name = product.Name,
                    IsDeleted = false,
                    ProductId = model.ProductId,
                    QuantityImport = model.QuantityImport,
                    WareHouse = wareHouse,
                    DateImport= DateTime.Now,
                };

                await _context.HistoryImports.AddAsync(history);

              


                await _context.SaveChangesAsync();
                return wareHouse;

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
        //dùng cho khi nhập hàng khi sp đã có trong kho => bấm vô button "nhập hàng"
        public async Task<dynamic> Update(WareHouseDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new WareHouseValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.WareHouses.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật


              
                //existingCustomer.Name = model.Name;
                //existingCustomer.ProductId = model.ProductId;
                existingCustomer.QuantityImport = model.QuantityImport;
                existingCustomer.RemainQuantity += model.QuantityImport;
                //existingCustomer.IsDeleted = model.IsDeleted;

                var product = await _context.Products.FindAsync(model.ProductId);
                if (product == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                var history = new HistoryImport()
                {
                    Name = product.Name,
                    IsDeleted = false,
                    ProductId = model.ProductId,
                    QuantityImport = model.QuantityImport,
                    WareHouseId = existingCustomer.Id,
                    DateImport = DateTime.Now,
                };

                await _context.HistoryImports.AddAsync(history);

                product.StockQuantity = existingCustomer.QuantityImport + product.StockQuantity;


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
