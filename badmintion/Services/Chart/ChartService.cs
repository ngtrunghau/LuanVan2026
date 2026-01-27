using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Chart;
using badmintion.Interface.Core;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using badmintion.Models.Chart;
using badmintion.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace badmintion.Services.Chart
{
    public class ChartService : IChartService
    {
        private readonly IUserService _userService;
        private readonly BadmintionNlContext _context;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly JwtSettings _jwtSettings;
        public ChartService(
            BadmintionNlContext context,
            IHttpContextAccessor contextAccessor,
            IRefreshTokenService refreshTokenService,
            IOptions<JwtSettings> jwtSettings,
            IUserService userService)
        {
            _context = context;
            _refreshTokenService = refreshTokenService;
            _jwtSettings = jwtSettings.Value;
            _userService = userService;
        }

        public async Task<dynamic> GetDoanhThuThang()
        {
            try
            {
                int year = int.Parse(DateTime.Now.Year.ToString());
                // Data chart
                List<DataChart> data = _context.Orders.Where(x => x.OrderDate.Value.Year == year && x.Status == 2).Include(x => x.OrderItems)
                    .GroupBy(x => x.OrderDate.Value.Month)
                    .Select(x => new DataChart()
                    {
                        Label = (x.FirstOrDefault().OrderDate.Value.Month),
                        Y = x.ToList().Sum(y => y.OrderItems.Sum(b => b.Quantity * b.Price)).Value
                    }).ToList();

                List<DataPoint> dataPoints = new List<DataPoint>();
                foreach (var item in data)
                {
                    var poin = new DataPoint()
                    {
                        Label = "Tháng " + item.Label,
                        Y = double.Parse(item.Y.ToString())

                    };
                    dataPoints.Add(poin);
                }
                return dataPoints;

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
