using framework.BaseService.BusinessServices;
using framework.BaseService.BusinessServices.Jwt;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Middlewares.Jwt;
using framework.BaseService.Models.Jwt;
using framework.BaseService.Repository;
using framework.GeneralSetting.BusinessServices.Modification;
using framework.GeneralSetting.BusinessServices.Retrieval;
using framework.GeneralSetting.Interfaces.ModificationService;
using framework.GeneralSetting.Interfaces.Retrieval;
using framework.Product.BusinessServices.Modification;
using framework.Product.BusinessServices.Retrieval;
using framework.Product.Interfaces.Modificaiton;
using framework.Product.Interfaces.Retrieval;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductManagement.DataAccess.DataContexts.GeneralSettings;
using ProductManagement.DataAccess.DataContexts.Products;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Host.UseSerilog();

// Load JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtModel>()
    ?? throw new Exception("JWT settings are missing in appsettings.json configuration.");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

// Configure authentication with JWT and Cookie Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.LoginPath = "/Account/Login"; // Redirect unauthorized users to login
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

// Register HttpClientFactory
builder.Services.AddHttpClient();

// Register Controllers & Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


// Swagger registration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ProductManagement API",
        Version = "v1",
        Description = "API documentation for ProductManagement"
    });

    // Enable JWT Authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}' in the field below (without quotes).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

// Database connection
var connectionString = builder.Configuration.GetConnectionString("ProductManagementDB");
builder.Services.AddDbContext<GeneralSettingContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register dependencies
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IRepository<GeneralSettingContext>, RepositoryImplementor<GeneralSettingContext>>();
builder.Services.AddScoped<IRepository<ProductContext>, RepositoryImplementor<ProductContext>>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefUserService, RefUserService>();
builder.Services.AddScoped<IRefUserGetService, RefUserGetService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductGetService, ProductGetService>();
builder.Services.AddScoped<GridPaginationService>();

// Bind JWT settings for DI
builder.Services.Configure<JwtModel>(builder.Configuration.GetSection("JwtSettings"));

var app = builder.Build();

//  Enable Serilog logging
app.UseSerilogRequestLogging();

// Enable static files (for Bootstrap & CSS)
app.UseStaticFiles();

// Enable routing
app.UseRouting();

// Enable Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers and Razor Pages
app.MapControllers();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Fix Swagger execution order
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Ensure Swagger is available in development
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManagement API v1");
    });
}

app.UseHttpsRedirection();

//Check database connection
using (var scope = app.Services.CreateScope())
{
    var generalSettingDbContext = scope.ServiceProvider.GetRequiredService<GeneralSettingContext>();
    var productDbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();

    try
    {
        Console.WriteLine("Checking database connection...");
        if (generalSettingDbContext.Database.CanConnect() && productDbContext.Database.CanConnect())
        {
            Console.WriteLine("Database connection successful!");
        }
        else
        {
            Console.WriteLine("Database connection failed.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection failed: {ex.Message}");
    }
}

app.Run();
