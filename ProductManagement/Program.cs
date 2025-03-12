using framework.BaseService.BusinessServices.Jwt;
using framework.BaseService.Controllers;
using framework.BaseService.Interfaces.Jwt;
using framework.BaseService.Middlewares.Jwt;
using framework.BaseService.Models.Jwt;
using framework.BaseService.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductManagement.DataAccess.DataContexts.GeneralSettings;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load JWT settings from configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtModel>()
    ?? throw new Exception("JWT settings are missing in configuration.");

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
});

// Database connection
var connectionString = builder.Configuration.GetConnectionString("ProductManagementDB");
builder.Services.AddDbContext<GeneralSettingContext>(options =>
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

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Scoped Dependencies
builder.Services.AddScoped<IRepository<GeneralSettingContext>, RepositoryImplementor<GeneralSettingContext>>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Bind JWT settings for DI
builder.Services.Configure<JwtModel>(builder.Configuration.GetSection("JwtSettings"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Check database connection
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<GeneralSettingContext>();

    try
    {
        Console.WriteLine("Checking database connection...");
        if (dbContext.Database.CanConnect())
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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductManagement API v1");
        c.DefaultModelsExpandDepth(-1); // This code for hide Schema section in swagger
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<JwtMiddleware>();

app.Run();