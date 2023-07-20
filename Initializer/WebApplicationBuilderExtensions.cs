using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Zack.EventBus;
using Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using JWT;
using ASPNETCore;
using Zack.Commons;
using Zack.Commons.JsonConverters;
using Serilog;
using FluentValidation.AspNetCore;
using FluentValidation;
using StackExchange.Redis;
using Microsoft.AspNetCore.HttpOverrides;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Initializer
{
    public static class WebApplicationBuilderExtensions
    {

        public static void ConfigureAppConfiguration(this WebApplicationBuilder builder)
        {
            string dir = builder.Configuration.GetValue<string>("DefaultAppSettings")!;
            string fullPath = Path.Combine(dir, "appsettings.json");
            builder.Configuration.AddJsonFile(fullPath);

        }


        //Serilog
        public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions)
        {
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;
            if (!string.IsNullOrEmpty(initOptions.LogFilePath))
            {
                services.AddLogging(builder =>
                {
                    Log.Logger = new LoggerConfiguration()
                       // .MinimumLevel.Information()
                       .Enrich.FromLogContext()
                       .WriteTo.Console()
                       .WriteTo.File(initOptions.LogFilePath)
                       .CreateLogger();
                    builder.AddSerilog();
                });

            }
            else
            {
                builder.Host.UseSerilog((context, services, configuration) => configuration
                           .ReadFrom.Configuration(context.Configuration)
                           .ReadFrom.Services(services)
                           .Enrich.FromLogContext()
                           .WriteTo.Console());
            }


            var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
            services.RunModuleInitializers(assemblies);

            //DbContexts
            services.AddAllDbContexts(ctx =>
            {
                string connectionStrings = configuration.GetValue<string>("DefaultDB:ConnectionStrings")!;
                ctx.UseSqlServer(connectionStrings);
            }, assemblies);

            services.AddAuthorization();
            services.AddAuthentication();
            JWTOptions jwtOpt = configuration.GetSection("JWT").Get<JWTOptions>()!;
            services.AddJWTAuthentication(jwtOpt);

            //启用Swagger中的【Authorize】按钮。
            builder.Services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddAuthenticationHeader();
            });


            services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(assemblies.ToArray()));
            //不用手动AddMVC了，因此把文档中的services.AddMvc(c =>{})改写成Configure<MvcOptions>(c=> {})这个问题很多都类似
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<UnitOfWorkFilter>();
            });
            services.Configure<JsonOptions>(options =>
            {
                //设置时间格式。而非“2008-08-08T08:08:08”这样的格式
                options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            });

            services.AddCors(options =>
            {
                //更好的在Program.cs中用绑定方式读取配置的方法：https://github.com/dotnet/aspnetcore/issues/21491
                //不过比较麻烦。
                var corsOpt = configuration.GetSection("Cors").Get<CorsSettings>()!;
                string[] urls = corsOpt.Origins;
                options.AddDefaultPolicy(builder => builder.WithOrigins(urls)
                        .AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            }
            );

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblies(assemblies);

            services.Configure<JWTOptions>(configuration.GetSection("JWT"));
            services.Configure<IntegrationEventRabbitMQOptions>(configuration.GetSection("RabbitMQ"));
            services.AddEventBus(initOptions.EventBusQueueName, assemblies);

            //Redis的配置
            string redisConnectionStrings = configuration.GetValue<string>("Redis:ConnectionStrings")!;
            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConnectionStrings);
            services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
        }


    }
}
