using badmintion.Contansts;
using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace badmintion.Services.Core
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly BadmintionNlContext _context;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IPasswordService _passwordService;
        private readonly JwtSettings _jwtSettings;
        public AuthService(
            BadmintionNlContext context, 
            IHttpContextAccessor contextAccessor,
            IRefreshTokenService refreshTokenService,
            IOptions<JwtSettings> jwtSettings, 
            IUserService userService,
            IPasswordService passwordService)
        {
            _context = context;
            _refreshTokenService = refreshTokenService;
            _jwtSettings = jwtSettings.Value;
            _userService = userService;
            _passwordService = passwordService;
        }

        public async Task<dynamic> Login(AuthRequest model)
        {
            try
            {
                if (model == default)
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

                ValidationResult validationResult = new AuthRequestValidation().Validate(model);
                if (!validationResult.IsValid)
                    throw new ResponseMessageException().WithValidationResult(validationResult);

                var login = await _context.Users.FirstOrDefaultAsync(
                    x => x.UserName == model.Username && x.IsDeleted == false);
                if (login == null || !_passwordService.Verify(model.Password, login.Password))
                {
                    throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

                }
                if (_passwordService.NeedsRehash(login.Password))
                {
                    login.Password = _passwordService.Hash(model.Password);
                    await _context.SaveChangesAsync();
                }
                if (login != null)
                {
                    var listFunc1 = await _context.FunctionManages.Where(x => x.IsDeleted == false && x.UnitRoleId == login.UnitRoleId).Include(o => o.Controller).ToListAsync();
                    var list = new List<ResponseLogin>();
                    foreach (var item in listFunc1)
                    {

                        var itemFunc = new ResponseLogin()
                        {
                            router = item.Router,
                            name = item.Name,
                            controller = item.Controller.Name,
                        };
                        list.Add(itemFunc);
                    }
                    return list;
                }

                return true;

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
        public async Task<dynamic> LoginAsync(AuthRequest model)
        {
            var user = await AuthenticateTest(model);
            if (user.IsDeleted == true)
            {
                throw new ResponseMessageException().WithException(DefaultCode.ACCOUNT_IS_LOCKED);
            }
            return await GenerateAuthenticationResultForUserAsync(user);
        }
        public async Task<User> AuthenticateTest(AuthRequest model)
        {
            if (model == default)
                throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);

            ValidationResult validationResult = new AuthRequestValidation().Validate(model);
            if (!validationResult.IsValid)
                throw new ResponseMessageException().WithValidationResult(validationResult);

            var login = await _context.Users
                .Include(x => x.UnitRole)
                .FirstOrDefaultAsync(x => x.UserName == model.Username && x.IsDeleted == false);
            if (login == null || !_passwordService.Verify(model.Password, login.Password))
            {
                throw new ResponseMessageException().WithException(DefaultCode.DATA_NOT_FOUND);

            }
            if (_passwordService.NeedsRehash(login.Password))
            {
                login.Password = _passwordService.Hash(model.Password);
                await _context.SaveChangesAsync();
            }
            var loginUser = await _context.Logins.Where(x => x.IsDeleted == false && x.UserId == login.Id).OrderByDescending(x => x.Date).FirstOrDefaultAsync();
            //var check =  _refreshTokenService.ValidateJwtToken(loginUser.AccessToken);
            //if (check == null) throw new ResponseMessageException().WithException(DefaultCode.REFRESH_TOKEN_OUT_TIME);
            return login;
        }



        public async Task<dynamic> GenerateAuthenticationResultForUserAsync(User user)
        {

            if (user == null) throw new ResponseMessageException().WithException(DefaultCode.EXCEPTION);


            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await _refreshTokenService.CreateJwtSecurityToken(tokenHandler, user);

            var refreshToken = new RefreshTokenDTO
            {
                JwtId = token.Id,
                UserId = user.Id,
                UnitRoleId = user.UnitRole.Id,
                AccessToken = tokenHandler.WriteToken(token),
                CreationDateToken = DateTime.UtcNow,
                ExpiryDateToken = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                ExpiryDateRefreshToken = FormatTime.ConvertToUnixTimestamp(DateTime.UtcNow.Add(_jwtSettings.TokenRefreshStore)),
                RefreshToken = _refreshTokenService.GenerateRefreshToken()
            };
            var userDTO = new UserDTO
            {
                Name = user.Name,
                UserName = user.UserName,
                Password = null,
                IsDeleted = user.IsDeleted,
                Id = user.Id,
                UnitRoleId = user.UnitRoleId,
            };
            var createdRefreshToken = await _refreshTokenService.Create(refreshToken);
            var userLogin = new UserLogin(userDTO, refreshToken.ExpiryDateToken);
            userLogin.AccessToken = refreshToken.AccessToken;
            userLogin.RefreshToken = refreshToken.RefreshToken;
            return userLogin;
        }


    }
}
