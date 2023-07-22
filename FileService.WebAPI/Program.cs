using FileService.Infrastructure.Services;
using Initializer;

var builder = WebApplication.CreateBuilder(args);




builder.ConfigureAppConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "FileService.WebAPI",
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FileService.WebAPI", Version = "v1" });
});

builder.Services.Configure<SMBStorageOptions>(builder.Configuration.GetSection("FileService:SMB"));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FileService.WebAPI v1"));
}

app.UseStaticFiles();
app.UseDefaultMiddleware();

app.MapControllers();

app.Run();
