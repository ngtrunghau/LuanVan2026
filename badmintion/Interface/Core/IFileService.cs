using badmintion.DTO;

namespace badmintion.Interface.Core
{
    public interface IFileService
    {
        Task<dynamic> Create(IFormFile files);
    }
}
