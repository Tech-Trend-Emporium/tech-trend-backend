using API.Middlewares;
using Application.Abstraction;
using Application.Abstractions;
using Application.Repository;
using Application.Services;
using Application.Services.Implementations;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Starter;

var builder = WebApplication.CreateBuilder(args);

// Configure PostgreSQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var products = await SeedFromApi.FetchProductsAsync();
SeedFromApi.AddCategoriesIfNotExistAsync(products, new AppDbContext(
    new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(connectionString)
        .Options)).Wait();
SeedFromApi.AddProductsIfNotExistAsync(products, new AppDbContext(
    new DbContextOptionsBuilder<AppDbContext>()
        .UseNpgsql(connectionString)
        .Options)).Wait();



// Configure JWT authentication
var jwt = builder.Configuration.GetSection("Jwt");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        o.Events = new JwtBearerEvents
        {
            OnTokenValidated = async ctx =>
            {
                var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var userIdStr = ctx.Principal!.FindFirstValue(ClaimTypes.NameIdentifier);
                var stamp = ctx.Principal!.FindFirstValue("ss");
                if (!int.TryParse(userIdStr, out var userId))
                {
                    ctx.Fail("Invalid UserId");
                    return;
                }
                var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null || !user.IsActive || user.SecurityStamp != stamp) ctx.Fail("Invalid token");
            }
        };
    });

// RBAC (roles and claims)
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("RequireAdmin", p => p.RequireRole(nameof(Domain.Enums.Role.ADMIN)));
    opt.AddPolicy("CanManageProducts", p => p.RequireClaim("perm", "products.manage"));
});

// Add repositories
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IApprovalJobRepository, ApprovalJobRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWishListRepository, WishListRepository>();

// Add services
builder.Services.AddScoped<IApprovalJobService, ApprovalJobService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IWishListService, WishListService>();

// Add controllers
builder.Services.AddControllers();

// Configure API versioning
var apiVersioningBuilder = builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = new UrlSegmentApiVersionReader(); 
});
apiVersioningBuilder.AddApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the HTTP request pipeline.
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
