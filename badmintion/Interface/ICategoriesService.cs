using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface ICategoriesService
    {
        Task<dynamic> Create(CategoriesDTO model);
        Task<dynamic> Update(CategoriesDTO model);
        Task<dynamic> GetAll();
    }
}
