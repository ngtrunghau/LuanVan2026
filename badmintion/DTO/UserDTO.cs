using badmintion.Models;

namespace badmintion.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? UserName { get; set; }

        public string? Password { get; set; }

        public bool? IsDeleted { get; set; }

        public int? UnitRoleId { get; set; }
    }
    public class AuthRequest
    {
        public string Username { get; set; }

        public string Password { get; set; }

    }
    public class ListAction
    {
        public string name { get; set; }

        public string router { get; set; }

    }

    public class ResponseLogin
    {
        public string username { get; set; }
        public string fullname { get; set; }
        public string controller { get; set; }
        public string name { get; set; }
        public string router { get; set; }
        //public int unitRoleId { get; set; }

    }

    public class JwtSettings
    {
        public string Secret { get; set; }
        public TimeSpan TokenLifetime { get; set; }
        public TimeSpan TokenRefreshStore { get; set; }
        public TimeSpan NormalTokenLife { get; set; }
        public TimeSpan PaygovTokenLife { get; set; }
        public TimeSpan OthersTokenLife { get; set; }

        public string KcnMgdK { get; set; }
    }
    public class UserLogin
    {
        public UserLogin() { }
        public UserLogin(UserDTO model, DateTime expiryDate)
        {
            this.Id = model.Id;
            this.UserName = model.UserName;
            this.UnitRoleId = model.UnitRoleId;
            this.FullName = model.Name;
            this.ExpiryDate = expiryDate;

        }
        public int Id { get; set; }
        public string UserName { get; set; }

        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public DateTime ExpiryDate { get; set; }
        public string FullName { get; set; }
        public int? UnitRoleId { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string Address { get; set; }
    }
}
