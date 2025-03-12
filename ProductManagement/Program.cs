
using framework.BaseService.BusinessServices;
using framework.BaseService.BusinessServices.Jwt;
using framework.BaseService.Controllers;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Middlewares.Jwt;
using framework.BaseService.Models.Jwt;
using framework.BaseService.Repository;
using framework.GeneralSetting.BusinessServices.Modification;
using framework.GeneralSetting.BusinessServices.Retrieval;
using framework.GeneralSetting.Controllers;
using framework.GeneralSetting.Interfaces.ModificationService;
using framework.GeneralSetting.Interfaces.Retrieval;
using framework.Product.BusinessServices.Modification;
using framework.Product.BusinessServices.Retrieval;
using framework.Product.Controllers;
using framework.Product.Interfaces.Modificaiton;
using framework.Product.Interfaces.Retrieval;
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
//Configure Serilog
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

// Convert the secret key to bytes
var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

// Configure authentication with JWT Bearer
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
    });

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddControllers().ConfigureApplicationPartManager(apm =>
{
    apm.ApplicationParts.Add(new AssemblyPart(typeof(AuthController).Assembly));
    apm.ApplicationParts.Add(new AssemblyPart(typeof(RefUserController).Assembly));
    apm.ApplicationParts.Add(new AssemblyPart(typeof(ProductController).Assembly));
});

// Database connection
var connectionString = builder.Configuration.GetConnectionString("ProductManagementDB");
builder.Services.AddDbContext<GeneralSettingContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDbContext<ProductContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Register dependencies
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();


// Add Swagger with JWT Support
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

//Register Serilog as the logger
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Scoped Dependencies
//DataContext
builder.Services.AddScoped<IRepository<GeneralSettingContext>, RepositoryImplementor<GeneralSettingContext>>();
builder.Services.AddScoped<IRepository<ProductContext>, RepositoryImplementor<ProductContext>>();

//Services
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IRefUserService, RefUserService>();
builder.Services.AddScoped<IRefUserGetService, RefUserGetService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductGetService, ProductGetService>();
builder.Services.AddScoped<GridPaginationService>();

// Bind JWT settings for DI
builder.Services.Configure<JwtModel>(builder.Configuration.GetSection("JwtSettings"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();
// Check database connection
using (var scope = app.Services.CreateScope())
{
    var generalSettingDbContext = scope.ServiceProvider.GetRequiredService<GeneralSettingContext>();
    var productDbContext = scope.ServiceProvider.GetRequiredService<ProductContext>();

    try
    {
        Console.WriteLine("Checking database connection...");
        if (generalSettingDbContext.Database.CanConnect() && productDbContext.Database.CanConnect())
        {
            Console.WriteLine("General Setting and Product connection successful!");
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManagement API v1");
        //c.DefaultModelsExpandDepth(-1); // This code for hide Schema section in swagger
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<JwtMiddleware>();


app.Run();