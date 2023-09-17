using Initializer;

var builder = WebApplication.CreateBuilder(args);

builder.ReadHostBuilderConfiguration();
builder.ConfigureCommonServices(new InitializerOptions
{
    LogFilePath = "h:/DDDDemoTemp/logs/Showcase.Main/log-.txt",
    EventBusQueueName = "Showcase.Main"
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Showcase.Main.WebAPI", Version = "v1" });
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
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Showcase.Main.WebAPI v1"));
}

//app.MapHub<GameEncodingStatusHub>("/Hubs/StatusHub");
app.UseCommonMiddlewares();

app.MapControllers();

app.Run();