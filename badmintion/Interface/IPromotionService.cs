using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface IPromotionService
    {
        Task<dynamic> Create(Promotion model);
        Task<dynamic> Update(Promotion model);
        Task<dynamic> GetPaging(PromotionPagingRequest model);
        Task<dynamic> SetActive(PromotionStatusRequest model);
        Task<Promotion?> GetValidPromotion(string code, decimal orderValue);
        decimal CalculateDiscount(Promotion promotion, decimal orderValue);
    }
}
