using BusinessLogic;
using Database.Implements;
using DataModels.Services;
using DataModels.Storages;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IUserService, UserLogic>();
builder.Services.AddTransient<IUserStorage, UserStorage>();
builder.Services.AddTransient<IMeetingService, MeetingLogic>();
builder.Services.AddTransient<IMeetingStorage, MeetingStorage>();
builder.Services.AddTransient<IMeetingUserService, MeetingUserLogic>();
builder.Services.AddTransient<IMeetingUserStorage, MeetingUserStorage>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API",
        Version = "v1"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
