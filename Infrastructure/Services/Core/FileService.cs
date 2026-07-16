using badmintion.Interface;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Validation;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services.Core
{
    public class FileService : IFileService
    {
        public async Task<dynamic> Create(IFormFile file)
        {
            try
            {
                if (file == null || (file != null && file.Length == 0))
                {
                    throw new ResponseMessageException()
                        .WithCode(DefaultCode.ERROR_STRUCTURE)
                        .WithMessage("File tải lên đang bị rỗng");
                }
                var fileName = Path.GetFileName(file.FileName);

                var account = new Account(
                   "dli65lc58",
                   "753878939239355",
                   "IcIr4PzPlEZ9LqCgNEoZcGEv2-I");

                var cloudinary = new Cloudinary(account);
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),

                    };
                    var uploadResult = cloudinary.Upload(uploadParams).Url.ToString();
                    //var urlImg = uploadResult["url"].Value;
                    
                    return uploadResult;
                }



            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }
    }
}
