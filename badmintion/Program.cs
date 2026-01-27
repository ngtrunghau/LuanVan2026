using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Chart;
using badmintion.Interface.Core;
using badmintion.Interface.Others;
using badmintion.Models;
using badmintion.Services;
using badmintion.Services.Chart;
using badmintion.Services.Core;
using badmintion.Services.Others;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VNPAY.NET;
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<BadmintionNlContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Cấu hình Swagger để hỗ trợ Bearer Token Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter your Bearer token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
});
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressCustomerService, AddressCustomerService>();
builder.Services.AddScoped<ICategoriesService, CategoriesService>();
builder.Services.AddScoped<IOdersService, OdersService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISalesReportService, SalesReportService>();
builder.Services.AddScoped<IShippingDetailService, ShippingDetailService>();
builder.Services.AddScoped<IControllerManageService, ControllerManageService>();
builder.Services.AddScoped<IFunctionManageService, FunctionManageService>();
builder.Services.AddScoped<IUnitRoleService, UnitRoleService>();
builder.Services.AddScoped<IWareHouseService, WareHouseService>();
builder.Services.AddScoped<IVnPayService, VnPayService>();
builder.Services.AddSingleton<IVnpay, Vnpay>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<ITownService, TownService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHistoryImportService, HistoryImportService>();
builder.Services.AddScoped<IChartService, ChartService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
// Thêm chính sách CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:8080") // Cho phép frontend truy cập
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddSingleton(new TokenValidationParameters
{
    // Cấu hình các tham số xác thực token tại đây
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero,  // Điều chỉnh nếu cần
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DongThap@123"))
});
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DongThap@123")),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.Events = new JwtBearerEvents
    {
        OnTokenValidated = async (context) =>
        {
            var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
            var userId = 1;
            /*var user = await userService.GetById(userId);
            if (user == null )
            {

                // return unauthorized if user no longer exists
                context.Fail("Unauthorized");

            }
            else
            {
                if(user != null)
                    context.HttpContext.Items["User"] = user;
            }*/
        }
    };
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = tokenValidationParameters;
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Sử dụng CORS trước khi định nghĩa các middleware khác
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();



app.Run();
