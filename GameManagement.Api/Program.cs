using GameManagement.Api.Authorization;
using GameManagement.Api.DataAccess;
using GameManagement.Api.Filters;
using GameManagement.Api.Services;
using GameManagement.Shared.DataAccess;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Serilog;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
}).AddNewtonsoftJson(setup =>
{
    setup.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
}).AddXmlDataContractSerializerFormatters()
.ConfigureApiBehaviorOptions(setup =>
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
    config.Filters.Add<JWTValidationFilter>();
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

    //options.AddAuthenticationHeader();

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddDbContext<GameManagementDbContext>(options =>
{
    //Get ConnectionStrings from secrets.json
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings").Value);
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<ApiIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings").Value);
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityCore<ApiIdentityUser>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
});

var idBuilder = new IdentityBuilder(typeof(ApiIdentityUser), typeof(ApiIdentityRole), builder.Services);
idBuilder.AddEntityFrameworkStores<ApiIdentityDbContext>()
    .AddDefaultTokenProviders()
    .AddRoleManager<RoleManager<ApiIdentityRole>>()
    .AddUserManager<UserManager<ApiIdentityUser>>();

//builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//.AddJwtBearer(options =>
//{
//    var jwtOpt = builder.Configuration.GetSection("JWT").Get<JWTOptions>();
//    byte[] keyBytes = Encoding.UTF8.GetBytes(jwtOpt!.Key!);
//    var secKey = new SymmetricSecurityKey(keyBytes);
//    options.TokenValidationParameters = new()
//    {
//        ValidateIssuer = false,
//        ValidateAudience = false,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = secKey
//    };
//});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Company", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("Manager", policy => policy.RequireRole("Administrator", "Manager"));
    options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator", "Manager", "Company"));

    //options.AddPolicy("Manager", policy => policy.RequireClaim("Manager"));
    //options.AddPolicy("Edit", policy => policy.RequireAssertion(context =>
    //{
    //    if (context.Company.HasClaim(options => options.Type == "Edit Albums"))
    //        return true;
    //    return false;
    //}));
    options.AddPolicy("Administrator2", policy => policy.AddRequirements(
         new EmailRequirement("@gmail.com"),
        new QualifiedUserRequirement()));
});

builder.Services.AddMemoryCache();
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
//app.UseCors();

app.UseResponseCaching();

app.UseRouting();

app.UseHttpCacheHeaders();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();