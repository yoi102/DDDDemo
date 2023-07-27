using Initializer;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppConfiguration();
builder.ConfigureExtraServices(new InitializerOptions
{
    LogFilePath = "h:/logs/Showcase/log-.txt",
    EventBusQueueName = "Showcase.Admin"
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Showcase.WebAPI", Version = "v1" });

});
builder.Services.AddSignalR();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Showcase.Admin.WebAPI v1"));

}
//app.MapHub<EpisodeEncodingStatusHub>("/Hubs/StatusHub");
app.UseDefaultMiddleware();


app.MapControllers();

app.Run();
