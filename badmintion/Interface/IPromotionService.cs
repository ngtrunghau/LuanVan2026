using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface IPromotionService
    {
        Task<dynamic> Create(Promotion model);
        Task<dynamic> Update(Promotion model);
    }
}
