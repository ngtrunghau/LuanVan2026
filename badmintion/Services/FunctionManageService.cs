using badmintion.Contansts;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class FunctionManageService : IFunctionManageService
    {
        private readonly BadmintionNlContext _context;

        public FunctionManageService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> Create(FunctionManageDTO model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new FunctionManageValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var func = new FunctionManage()
                {
                    Name = model.Name,
                    Router = model.Router,
                    ControllerId = model.ControllerId,
                    UnitRoleId = model.UnitRoleId,
                    Key = model.Key,
                    IsDeleted = model.IsDeleted == null ? false : model.IsDeleted,
                };

                await _context.FunctionManages.AddAsync(func);

                await _context.SaveChangesAsync();
                return func;

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

        //public Task<object> GetListActionAPI()
        //{
        //    var list = ListActionDefault.listActionAPI;
        //    return list;
        //}

        public async Task<dynamic> GetPagingCore(PagingParamDefault pagingParam)
        {
            try
            {
               
                PagingModel<dynamic> result = new PagingModel<dynamic>();
                var data = await _context.FunctionManages.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Id).Include(x => x.UnitRole).Include(x => x.Controller).Select(x => new
                {
                    x.Id,  // Thêm các trường bạn cần từ FunctionManages
                    x.Name,
                    x.Router,
                    x.Controller ,
                    UnitRole = new
                    {
                        x.UnitRole.Id,
                        x.UnitRole.Name,
                        // Các trường cần thiết từ UnitRole
                    }
                }).Skip(pagingParam.Skip).Take(pagingParam.Limit).ToListAsync();

                result.Data = data;
                result.TotalRows =  _context.FunctionManages.Where(x => x.IsDeleted == false).Count();
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

        public async Task<dynamic> Update(FunctionManageDTO model)
        {
            try
            {
                // Kiểm tra nếu đối tượng model là null hoặc không hợp lệ
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                // Validate dữ liệu trước khi thực hiện cập nhật
                ValidationResult validationResult = new FunctionManageValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                // Tìm kiếm đối tượng Customer trong cơ sở dữ liệu theo ID
                var existingCustomer = await _context.FunctionManages.FindAsync(model.Id);
                if (existingCustomer == null)
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);  // Nếu không tìm thấy đối tượng cần cập nhật

                existingCustomer.Name = model.Name;
                existingCustomer.Key = model.Key;
                existingCustomer.Router = model.Router;
                existingCustomer.ControllerId = model.ControllerId;
                existingCustomer.UnitRoleId = model.UnitRoleId;
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
