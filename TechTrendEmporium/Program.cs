using API.Filters;
using API.Middlewares;
using Application.Abstraction;
using Application.Abstractions;
using Application.Repository;
using Application.Services;
using Application.Services.Implementations;
using Asp.Versioning;
using Azure.Identity;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Starter;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction())
{
    // Load secrets from Azure Key Vault in production
    var kvUrl = Environment.GetEnvironmentVariable("KEYVAULT_URL") ?? "https://ttekv.vault.azure.net/";
    builder.Configuration.AddAzureKeyVault(new Uri(kvUrl), new DefaultAzureCredential());
}

// Configure PostgreSQL connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection Missed");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString, npg => npg.EnableRetryOnFailure(5)));

// Configure Serilog
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Configure JWT authentication
var jwt = builder.Configuration.GetSection("Jwt");
var signingKeyValue = jwt["SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey Missed");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyValue));

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

// Add health checks
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy()).AddDbContextCheck<AppDbContext>(name: "db");

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.OperationFilter<AuthorizeCheckOperationFilter>();
});

// Configure the HTTP request pipeline.
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    await ctx.Database.MigrateAsync();

    var products = await SeedFromApi.FetchProductsAsync();
    await SeedFromApi.AddCategoriesIfNotExistAsync(products, ctx);

    await Task.Delay(TimeSpan.FromSeconds(1)); // Small delay to avoid 403 error from external API

    await SeedFromApi.AddProductsIfNotExistAsync(products, ctx);

    await Task.Delay(TimeSpan.FromSeconds(1));

    var users = await SeedFromApi.FetchUsersAsync(); 
    await SeedFromApi.AddUsersIfNotExistAsync(users, ctx, hasher);
}

/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/

app.UseSwagger();
app.UseSwaggerUI(); // Remove and uncomment above in production if needed


// Enforce HTTPS in production
var useHttps = builder.Configuration.GetValue<bool>("UseHttps", false);
if (useHttps) { app.UseHsts(); app.UseHttpsRedirection(); }

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = r => r.Name == "self" });
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
});

app.Run();