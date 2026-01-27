using badmintion.Models.Others;

namespace badmintion.Interface.Others
{
    public interface IVnPayService
    {
        Task<dynamic> CreatePaymentUrl(double money, int id);
        Task<dynamic> GetPaymentResult(IQueryCollection parameters);
    }
}
