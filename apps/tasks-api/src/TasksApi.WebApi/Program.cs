using TasksApi.Application.Commands;
using TasksApi.Application.Interfaces;
using TasksApi.Application.Queries;
using TasksApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

