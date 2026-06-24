using badmintion.DTO;
using badmintion.Interface;
using badmintion.Interface.Chart;
using badmintion.Interface.Core;
using badmintion.Models;
using badmintion.Services;
using badmintion.Services.Chart;
using badmintion.Services.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
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
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddScoped<IProvinceService, ProvinceService>();
builder.Services.AddScoped<IDistrictService, DistrictService>();
builder.Services.AddScoped<ITownService, TownService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddSingleton<IPasswordService, PasswordService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAccountSecurityService, AccountSecurityService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHistoryImportService, HistoryImportService>();
builder.Services.AddScoped<IChartService, ChartService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IAssistantService, AssistantService>();
// Thêm chính sách CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins(
                    "http://localhost:8080",
                    "http://localhost:8081",
                    "https://localhost:8080",
                    "https://localhost:8081"
                ) // Cho phép frontend truy cập
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    if (!builder.Environment.IsDevelopment())
    {
        throw new InvalidOperationException(
            "Thiếu cấu hình JwtSettings:Secret. Hãy cung cấp bằng biến môi trường hoặc secret store.");
    }

    jwtSecret = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(48));
}
builder.Configuration["JwtSettings:Secret"] = jwtSecret;

var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = true,
    ValidateLifetime = true,
    ClockSkew = TimeSpan.Zero
};
builder.Services.AddSingleton(tokenValidationParameters);


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
            var principal = context.Principal;
            var userIdValue = principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                              ?? principal?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var issuedAtValue = principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat)?.Value;
            if (!int.TryParse(userIdValue, out var userId))
            {
                context.Fail("Unauthorized");
                return;
            }

            var dataContext = context.HttpContext.RequestServices
                .GetRequiredService<BadmintionNlContext>();
            DateTime? issuedAt = long.TryParse(issuedAtValue, out var issuedAtSeconds)
                ? DateTimeOffset.FromUnixTimeSeconds(issuedAtSeconds).UtcDateTime
                : null;
            var isCustomer = string.Equals(
                principal?.FindFirst("account_type")?.Value,
                "customer",
                StringComparison.OrdinalIgnoreCase);
            DateTime? passwordChangedAt;
            if (isCustomer)
            {
                passwordChangedAt = await dataContext.Customers
                    .Where(x => x.Id == userId && x.IsDeleted == false)
                    .Select(x => x.PasswordChangedAt)
                    .FirstOrDefaultAsync();
            }
            else
            {
                passwordChangedAt = await dataContext.Users
                    .Where(x => x.Id == userId && x.IsDeleted == false)
                    .Select(x => x.PasswordChangedAt)
                    .FirstOrDefaultAsync();
            }

            if (passwordChangedAt.HasValue &&
                (!issuedAt.HasValue ||
                 issuedAt.Value.AddSeconds(2) < passwordChangedAt.Value))
            {
                context.Fail("Password changed after token issuance.");
            }
        }
    };
    x.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
    x.SaveToken = true;
    x.TokenValidationParameters = tokenValidationParameters;
});

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//});
var app = builder.Build();
await app.EnsureApplicationSchemaAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Sử dụng CORS trước khi định nghĩa các middleware khác
app.UseCors(MyAllowSpecificOrigins);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();
