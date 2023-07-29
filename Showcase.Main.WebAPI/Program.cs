using Initializer;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "h:/logs/Showcase.Main/log-.txt",
    EventBusQueueName = "Showcase.Main"
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Showcase.Main.WebAPI", Version = "v1" });
    c.EnableAnnotations();
});
builder.Services.AddSignalR();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Showcase.Main.WebAPI v1"));
}

//app.MapHub<EpisodeEncodingStatusHub>("/Hubs/StatusHub");
app.UseDefaultMiddleware();

app.MapControllers();

app.Run();