using GameManagement.Api.Authorization;
using GameManagement.Api.DataAccess;
using GameManagement.Api.Services;
using GameManagement.Shared.DataAccess;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddResponseCaching();
//Global Cache Configuration
builder.Services.AddHttpCacheHeaders(expires =>
{
    expires.MaxAge = 60;
    expires.CacheLocation = CacheLocation.Private;
}, validation =>
{
    validation.MustRevalidate = true;
});

// Add services to the container.
builder.Services.AddControllers(setup =>
{
    //CacheProfiles
    setup.CacheProfiles.Add("120sCacheProfile", new CacheProfile
    {
        Duration = 120
    });
    setup.ReturnHttpNotAcceptable = true;
    //setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
    //setup.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
}).AddNewtonsoftJson(setup =>
{
    setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
}).AddXmlDataContractSerializerFormatters()// XML ���ݩ`��׷��
.ConfigureApiBehaviorOptions(setup =>// Model ��٥�� Error
{
    setup.InvalidModelStateResponseFactory = context =>
    {
        // ErrorMessage 追加
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Type = "https://www.google.com/",//��д��
            Title = "Error が発生しました",
            Status = StatusCodes.Status422UnprocessableEntity,
            Detail = "  Google で検索",
            Instance = context.HttpContext.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);

        return new UnprocessableEntityObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    };
});

builder.Services.Configure<MvcOptions>(config =>
{
    var newtonSoftJsonOutputFormatter =
        config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

    newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.company.hateoas+json");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(options =>
{
    //注释
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //第二个参数为是否显示控制器注释,选择true
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename), true);

    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
     {
         Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
         In = ParameterLocation.Header,
         Name = "Authorization",
         Type = SecuritySchemeType.ApiKey
     });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<GameManagementDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<ApiIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApiIdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
}).AddEntityFrameworkStores<ApiIdentityDbContext>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Administrator", "Manager"));
    options.AddPolicy("Company", policy => policy.RequireRole("Administrator", "Manager", "Company"));

    //options.AddPolicy("Manager", policy => policy.RequireClaim("Manager"));
    //options.AddPolicy("Edit", policy => policy.RequireAssertion(context =>
    //{
    //    if (context.Company.HasClaim(x => x.Type == "Edit Albums"))
    //        return true;
    //    return false;
    //}));
    options.AddPolicy("Administrator2", policy => policy.AddRequirements(
         new EmailRequirement("@gmail.com"),
        new QualifiedUserRequirement()));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value!)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>();
builder.Services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();

builder.Services.AddSingleton<IAuthorizationHandler, EmailHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanEditHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AdministratorsHandler>();

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var app = builder.Build();

app.UseSerilogRequestLogging(); // <-- Add this line

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Unexpected Error!");//Logger......
        });
    });
}
//app.UseResponseCaching();

app.UseHttpCacheHeaders();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();