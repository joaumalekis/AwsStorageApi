using AwsStorageApi.Models;
using AwsStorageApi.Services;
using AwsStorageApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AwsConfiguration>(builder.Configuration.GetSection(nameof(AwsConfiguration)));
builder.Services.AddSingleton<IStorageService, StorageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
