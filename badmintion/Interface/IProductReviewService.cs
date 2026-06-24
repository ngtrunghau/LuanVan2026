using badmintion.DTO;

namespace badmintion.Interface
{
    public interface IProductReviewService
    {
        Task<dynamic> Create(ProductReviewDTO model);
        Task<dynamic> Update(ProductReviewDTO model);
        Task<dynamic> GetModerationList(ProductReviewFilterDTO model);
        Task<dynamic> Moderate(ProductReviewModerationDTO model);
        Task<dynamic> GetApprovedByProduct(int productId);
    }
}
