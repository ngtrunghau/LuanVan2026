using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Chart;
using badmintion.Interface.Core;
using badmintion.Models;
using badmintion.Services;
using badmintion.Services.Chart;
using badmintion.Services.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace badmintion.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<BadmintionNlContext>(option =>
                option.UseSqlServer(configuration.GetConnectionString("dbconn")));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IAddressCustomerService, AddressCustomerService>();
            services.AddScoped<ICategoriesService, CategoriesService>();
            services.AddScoped<IOdersService, OdersService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ISalesReportService, SalesReportService>();
            services.AddScoped<IShippingDetailService, ShippingDetailService>();
            services.AddScoped<IControllerManageService, ControllerManageService>();
            services.AddScoped<IFunctionManageService, FunctionManageService>();
            services.AddScoped<IUnitRoleService, UnitRoleService>();
            services.AddScoped<IWareHouseService, WareHouseService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddScoped<IProvinceService, ProvinceService>();
            services.AddScoped<IDistrictService, DistrictService>();
            services.AddScoped<ITownService, TownService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddSingleton<IPasswordService, PasswordService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAccountSecurityService, AccountSecurityService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IHistoryImportService, HistoryImportService>();
            services.AddScoped<IChartService, ChartService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<IPromotionService, PromotionService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IAssistantService, AssistantService>();

            return services;
        }
    }
}
