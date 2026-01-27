using Azure.Core;
using badmintion.Interface;
using badmintion.Interface.Others;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Models.Others;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace badmintion.Services.Others
{
    public class VnPayService : IVnPayService
    {
        private readonly IVnpay _vnpay;
        private readonly BadmintionNlContext _context;
        private readonly IConfiguration _configuration;
        private VnpayTest _apiSync;
        public VnPayService(BadmintionNlContext dataContext, IVnpay vnPayservice, IConfiguration configuration)
        {
            _vnpay = vnPayservice;
            _context = dataContext;
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration _configuration = builder.Build();
            _apiSync = _configuration.GetSection("Vnpay").Get<VnpayTest>();
            //_configuration = configuration;
            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
        }


        public async Task<dynamic> CreatePaymentUrl(double money, int id)
        {

            try
            {
                var ipAddress = "127.0.0.1"; // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

                var request = new PaymentRequest
                {
                    PaymentId = DateTime.Now.Ticks,
                    Money = money,
                    Description = "thanhtoandonhang",
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };
                var item = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
                item.TxnRef = request.PaymentId.ToString();
                await _context.SaveChangesAsync();
                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return paymentUrl;
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

        public  async Task<dynamic> GetPaymentResult(IQueryCollection parameters)
        {
            try
            {
                var paymentResult = _vnpay.GetPaymentResult(parameters);
                if (paymentResult.IsSuccess)
                {
                    var item =  _context.Orders.Include(x => x.OrderItems).ThenInclude(x => x.Products).ThenInclude(x => x.WareHouses).FirstOrDefault(o => o.TxnRef == paymentResult.PaymentId.ToString());
                    item.Status = 2;

                  
                    foreach (var item1 in item.OrderItems)
                    {
                        var prod = item1.Products;
                        prod.StockQuantity = prod.StockQuantity - item1.Quantity;


                       
                        foreach (var item2 in prod.WareHouses)
                        {
                           
                            item2.RemainQuantity = item2.RemainQuantity - item1.Quantity;
                        }


                    }

                   await _context.SaveChangesAsync();
                    //return RedirectToActionResult(
                    // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                    return true;
                }

                return false;
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
