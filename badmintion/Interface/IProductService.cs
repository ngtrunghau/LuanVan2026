using badmintion.DTO;
using badmintion.Lib.Core;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface IProductService
    {
        Task<dynamic> Create(ProductDTO model);
        Task<dynamic> Update(ProductDTO model);
        Task<dynamic> GetByIdCate(PagingParam id);
    }
}
