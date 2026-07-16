using badmintion.DTO;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services.Core
{
    public class AccountSecurityService : IAccountSecurityService
    {
        private readonly BadmintionNlContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IPasswordService _passwordService;

        public AccountSecurityService(
            BadmintionNlContext context,
            ICurrentUserService currentUser,
            IPasswordService passwordService)
        {
            _context = context;
            _currentUser = currentUser;
            _passwordService = passwordService;
        }

        public async Task ChangePassword(ChangePasswordDTO model)
        {
            Validate(model);

            if (!_currentUser.IsAuthenticated || !_currentUser.UserId.HasValue)
            {
                throw new ResponseMessageException()
                    .WithException(DefaultCode.NOT_HAVE_ACCESS);
            }

            if (_currentUser.IsCustomer)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(x =>
                    x.Id == _currentUser.UserId.Value &&
                    x.IsDeleted == false);
                if (customer == null)
                {
                    throw new ResponseMessageException()
                        .WithException(DefaultCode.DATA_NOT_FOUND);
                }

                EnsureCurrentPassword(model, customer.Password);
                customer.Password = _passwordService.Hash(model.NewPassword);
                customer.PasswordChangedAt = DateTime.UtcNow;
            }
            else
            {
                var user = await _context.Users.FirstOrDefaultAsync(x =>
                    x.Id == _currentUser.UserId.Value &&
                    x.IsDeleted == false);
                if (user == null)
                {
                    throw new ResponseMessageException()
                        .WithException(DefaultCode.DATA_NOT_FOUND);
                }

                EnsureCurrentPassword(model, user.Password);
                user.Password = _passwordService.Hash(model.NewPassword);
                user.PasswordChangedAt = DateTime.UtcNow;

                var logins = await _context.Logins
                    .Where(x => x.UserId == user.Id && x.IsDeleted == false)
                    .ToListAsync();
                foreach (var login in logins)
                {
                    login.IsDeleted = true;
                }

                var refreshTokens = await _context.RefreshTokens
                    .Where(x => x.UserId == user.Id &&
                                x.IsDeleted == false &&
                                x.Invalidated != true)
                    .ToListAsync();
                foreach (var refreshToken in refreshTokens)
                {
                    refreshToken.Invalidated = true;
                }
            }

            await _context.SaveChangesAsync();
        }

        private void EnsureCurrentPassword(
            ChangePasswordDTO model,
            string? storedPassword)
        {
            if (!_passwordService.Verify(model.CurrentPassword, storedPassword))
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Mật khẩu hiện tại không chính xác.");
            }

            if (_passwordService.Verify(model.NewPassword, storedPassword))
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Mật khẩu mới phải khác mật khẩu hiện tại.");
            }
        }

        private static void Validate(ChangePasswordDTO model)
        {
            if (model == null ||
                string.IsNullOrWhiteSpace(model.CurrentPassword) ||
                string.IsNullOrWhiteSpace(model.NewPassword) ||
                string.IsNullOrWhiteSpace(model.ConfirmPassword))
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Vui lòng nhập đầy đủ thông tin đổi mật khẩu.");
            }

            if (!string.Equals(
                    model.NewPassword,
                    model.ConfirmPassword,
                    StringComparison.Ordinal))
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Xác nhận mật khẩu mới không khớp.");
            }

            if (model.NewPassword.Length < 8 ||
                !model.NewPassword.Any(char.IsLetter) ||
                !model.NewPassword.Any(char.IsDigit))
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Mật khẩu mới phải có ít nhất 8 ký tự, gồm chữ và số.");
            }
        }
    }
}
