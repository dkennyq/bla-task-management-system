using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using UsersApi.Application.Commands;
using UsersApi.Application.Queries;
using UsersApi.Application.Services;
using UsersApi.Domain.Interfaces;
using UsersApi.Infrastructure.Repositories;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
        .AddEnvironmentVariables()
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .CreateLogger();

try
{
    Log.Information("Starting Users API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
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
                Array.Empty<string>()
            }
        });
    });

    // JWT Configuration
    var jwtSection = builder.Configuration.GetSection("Jwt");
    builder.Services.Configure<JwtSettings>(jwtSection);

    var jwtSettings = jwtSection.Get<JwtSettings>()
        ?? throw new InvalidOperationException("JWT configuration section is missing");

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetEffectiveSecretKey())),
            ClockSkew = TimeSpan.Zero
        };
    });

// PostgreSQL configuration
var connectionString = builder.Configuration.GetValue<string>("Postgres:ConnectionString")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=usersdb;Username=admin;Password=admin123;Include Error Detail=true";

// Register dependencies
builder.Services.AddScoped<IUserRepository>(sp =>
    new UserRepository(connectionString));
builder.Services.AddScoped<IRefreshTokenRepository>(sp =>
    new RefreshTokenRepository(connectionString));
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRegisterUserCommandHandler, RegisterUserCommandHandler>();
builder.Services.AddScoped<IGetCurrentUserQueryHandler, GetCurrentUserQueryHandler>();
builder.Services.AddScoped<IGetUsersQueryHandler, GetUsersQueryHandler>();
builder.Services.AddScoped<IUpdateUserCommandHandler, UpdateUserCommandHandler>();
builder.Services.AddScoped<IResetPasswordCommandHandler, ResetPasswordCommandHandler>();
builder.Services.AddScoped<ICreateUserByAdminCommandHandler, CreateUserByAdminCommandHandler>();
builder.Services.AddScoped<IUpdateUserRoleCommandHandler, UpdateUserRoleCommandHandler>();

    // CORS (for development)
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.UseMiddleware<UsersApi.WebApi.Middleware.ExceptionHandlingMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseCors("AllowAll");

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Users API listening on: {Urls}", string.Join(", ", app.Urls));

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Users API terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
