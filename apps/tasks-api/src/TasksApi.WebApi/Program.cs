using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TasksApi.Application.Commands;
using TasksApi.Application.Interfaces;
using TasksApi.Application.Queries;
using TasksApi.Infrastructure.Repositories;

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
    Log.Information("Starting Tasks API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
    var jwtSecret = builder.Configuration.GetValue<string>("Jwt:Secret")
        ?? builder.Configuration.GetValue<string>("Jwt:SecretKey")
        ?? throw new InvalidOperationException("JWT Secret is not configured");
    var jwtIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer") ?? "TaskManagementAPI";
    var jwtAudience = builder.Configuration.GetValue<string>("Jwt:Audience") ?? "TaskManagementWeb";

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
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

    // MongoDB configuration from environment variables with defaults
    var mongoConnectionString = builder.Configuration.GetValue<string>("MongoDB:ConnectionString") 
        ?? "mongodb://mongodb:27017";
    var mongoDatabaseName = builder.Configuration.GetValue<string>("MongoDB:DatabaseName") 
        ?? "tasksdb";

    // Register dependencies
    builder.Services.AddScoped<ITaskRepository>(sp => 
        new MongoTaskRepository(mongoConnectionString, mongoDatabaseName));
    builder.Services.AddScoped<GetAllTasksQueryHandler>();
    builder.Services.AddScoped<GetTaskByIdQueryHandler>();
    builder.Services.AddScoped<ICreateTaskCommandHandler, CreateTaskCommandHandler>();
    builder.Services.AddScoped<IUpdateTaskCommandHandler, UpdateTaskCommandHandler>();
    builder.Services.AddScoped<IDeleteTaskCommandHandler, DeleteTaskCommandHandler>();

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
    app.UseMiddleware<TasksApi.WebApi.Middleware.ExceptionHandlingMiddleware>();

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

    Log.Information("Tasks API listening on: {Urls}", string.Join(", ", app.Urls));

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Tasks API terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}
