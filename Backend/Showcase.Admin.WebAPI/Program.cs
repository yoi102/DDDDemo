using Initializer;
using Showcase.Admin.WebAPI.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.ReadHostBuilderConfiguration();
builder.ConfigureCommonServices(new InitializerOptions
{
    LogFilePath = "h:/DDDDemoTemp/logs/Showcase.Admin/log-.txt",
    EventBusQueueName = "Showcase.Admin"
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Showcase.Admin.WebAPI", Version = "v1" });
    c.EnableAnnotations();
});
builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Showcase.Admin.WebAPI v1"));
}
app.MapHub<StatusHub>("/Hubs/StatusHub");
app.UseCommonMiddleware();

app.MapControllers();

app.Run();