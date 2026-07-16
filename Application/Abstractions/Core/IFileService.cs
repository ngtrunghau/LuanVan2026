using badmintion.DTO;

using Microsoft.AspNetCore.Http;

namespace badmintion.Interface.Core
{
    public interface IFileService
    {
        Task<dynamic> Create(IFormFile files);
    }
}
