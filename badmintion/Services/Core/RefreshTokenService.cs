using badmintion.Contansts;
using badmintion.DTO;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace badmintion.Services.Core
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly BadmintionNlContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            TokenValidationParameters tokenValidationParameters,
            IOptions<JwtSettings> jwtSettings
 
            )
        {
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<dynamic> Create(RefreshTokenDTO model)
        {
            if (model == default)
            {
                throw new ResponseMessageException().WithException(DefaultCode.EXCEPTION);
            }
            var login = new Login()
            {
                UserId = model.UserId,
                AccessToken = model.AccessToken,
                RefreshToken = model.RefreshToken,
                Date = DateTime.Now,
                IsDeleted = false
            };
            await _context.Logins.AddAsync(login);

            await _context.SaveChangesAsync();
            return login;
        }

        public async Task<SecurityToken> CreateJwtSecurityToken(JwtSecurityTokenHandler tokenHandler, User user)
        {
            var secret = _jwtSettings.Secret;
            if (string.IsNullOrWhiteSpace(secret))
            {
                throw new InvalidOperationException("Thiếu cấu hình JwtSettings:Secret.");
            }
            var key = Encoding.ASCII.GetBytes(secret);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UnitRoleId.ToString()),
                new Claim("account_type", "admin"),
            };

            /*new Claim("UserName", userName),
            new Claim("Id", id),*/



            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = now,
                IssuedAt = now,
                Expires = now.Add(_jwtSettings.TokenLifetime),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

            };
            var token =  tokenHandler.CreateToken(tokenDescriptor);

            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<dynamic> RefreshToken(TokenApiModel model)
        {
            if (model == default)
                throw new ResponseMessageException().WithException(DefaultCode.EXCEPTION);

            var resultRefreshToken = ValidateJwtToken(model.AccessToken);

            if (resultRefreshToken == null || resultRefreshToken.Username == null || resultRefreshToken.UserId == null)
                throw new ResponseMessageException().WithException(DefaultCode.TOKEN_NOT_FOUND);


            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.IsDeleted == false
                                                               && x.Invalidated == false &&
                                                               x.RefreshToken1 == model.RefreshToken &&
                                                               x.AccessToken == model.AccessToken &&
                                                               x.UserId == Int32.Parse(resultRefreshToken.UserId));

            if (refreshToken == null)
                throw new ResponseMessageException().WithException(DefaultCode.TOKEN_OR_REFRESH_TOKEN_NOT_FOUND);

            var today = FormatTime.ConvertToUnixTimestamp(DateTime.UtcNow);

            if (refreshToken.Expirydaterefreshtoken - today < 0)
            {
                throw new ResponseMessageException().WithException(DefaultCode.REFRESH_TOKEN_OUT_TIME);
            }


            var user = await _context.Users.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == refreshToken.UserId);

            if (user == null)
                throw new ResponseMessageException().WithException(DefaultCode.EXCEPTION);
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = await CreateJwtSecurityToken(tokenHandler, user);

            if (token == null || token.Equals(""))
                throw new ResponseMessageException().WithException(DefaultCode.EXCEPTION);


            var refreshTokenModel = new RefreshToken
            {
                JwtId = token.Id,
                UserId = Int32.Parse(resultRefreshToken.UserId),
                AccessToken = tokenHandler.WriteToken(token),
                CreationDatetoken = DateTime.UtcNow,
                ExpiryDatetoken = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                Expirydaterefreshtoken = FormatTime.ConvertToUnixTimestamp(DateTime.UtcNow.Add(_jwtSettings.TokenRefreshStore)),
                RefreshToken1 = GenerateRefreshToken()
            };

            if (refreshTokenModel.AccessToken == null || refreshTokenModel.RefreshToken1 == null)
                throw new ResponseMessageException().WithException(DefaultCode.EXCEPTION);
            var refreshTokenModel1 = new RefreshTokenDTO()
            {
                UserId = user.Id,
                AccessToken = model.AccessToken,
                RefreshToken = model.RefreshToken,
            };
            var createdRefreshToken = Create(refreshTokenModel1);

            return new RefreshTokenResultModel(refreshTokenModel.AccessToken, refreshTokenModel.RefreshToken1, refreshToken.ExpiryDatetoken);

        }

        public ResultRefreshToken? ValidateJwtToken(string token)
        {
            if (token == null || token.Equals(""))
                return null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;
                //DateTime? expirationTime = tokenS.ValidTo;
                //if (expirationTime.HasValue && expirationTime.Value < DateTime.UtcNow)
                //{
                //    return null;
                //}
                var result = new ResultRefreshToken()
                {
                    UserId = tokenS.Claims.First(claim => claim.Type == ListActionDefault.KeyId).Value,
                    UnitRoleId = tokenS.Claims.First(claim => claim.Type == ListActionDefault.UnitRoleIdString).Value,

                };
                return result;
            }
            catch
            {
                return null;
            }
        }

        public dynamic ValidateExpirationTime(string token)
        {
            if (token == null || token.Equals(""))
                return null;
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token);
                var tokenS = jsonToken as JwtSecurityToken;
                DateTime? expirationTime = tokenS.ValidTo;
                if (expirationTime.HasValue && expirationTime.Value < DateTime.UtcNow)
                {
                    return null;
                }
                
                return true;
            }
            catch
            {
                return null;
            }
        }
    }
}
