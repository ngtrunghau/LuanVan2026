using badmintion.DTO;

namespace badmintion.Interface
{
    public interface IProductReviewService
    {
        Task<dynamic> Create(ProductReviewDTO model);
        Task<dynamic> Update(ProductReviewDTO model);
    }
}
