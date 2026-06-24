namespace badmintion.Interface.Core
{
    public interface ICurrentUserService
    {
        bool IsAuthenticated { get; }
        bool IsCustomer { get; }
        int? UserId { get; }
        string? UserName { get; }
    }
}
