using Initializer;
using SearchService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.ReadHostBuilderConfiguration();
builder.ConfigureCommonServices(new InitializerOptions
{
    LogFilePath = "h:/logs/SearchService.WebAPI/log-.txt",
    EventBusQueueName = "SearchService.WebAPI"
});
builder.Services.Configure<ElasticSearchOptions>(builder.Configuration.GetSection("ElasticSearch"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "SearchService.WebAPI", Version = "v1" });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SearchService.WebAPI v1"));
}

app.UseCommonMiddlewares();

app.MapControllers();

app.Run();