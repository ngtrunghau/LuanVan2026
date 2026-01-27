using badmintion.DTO;
using badmintion.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace badmintion.Interface.Core
{
    public interface IRefreshTokenService
    {
        Task<dynamic> Create(RefreshTokenDTO model);




        Task<SecurityToken> CreateJwtSecurityToken(JwtSecurityTokenHandler tokenHandler, User user);


        Task<dynamic> RefreshToken(TokenApiModel model);


        public string GenerateRefreshToken();


        public ResultRefreshToken? ValidateJwtToken(string token);

    }
}
