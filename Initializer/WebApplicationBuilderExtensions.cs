using ASPNETCore;
using Commons;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.EFCore;
using JWT;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerGen;
using Zack.EventBus;

namespace Initializer
{
    public static class WebApplicationBuilderExtensions
    {
        public static void ConfigureAppConfiguration(this WebApplicationBuilder builder)
        {
            var dir = builder.Configuration.GetValue<string>("DefaultDirectory");
            ArgumentException.ThrowIfNullOrEmpty(dir, "DefaultDirectory");

            string fullPath = Path.Combine(dir, "appsettings.json");
            builder.Configuration.AddJsonFile(fullPath);
        }

        //Serilog
        public static void ConfigureExtraServices(this WebApplicationBuilder builder, InitializerOptions initOptions)
        {
            IServiceCollection services = builder.Services;
            IConfiguration configuration = builder.Configuration;

            builder.Host.UseSerilog((context, services, configuration) => configuration
                       .ReadFrom.Configuration(context.Configuration)
                       .WriteTo.File(initOptions.LogFilePath)
                       .ReadFrom.Services(services)
                       .Enrich.FromLogContext()
                       .WriteTo.Console());

            var assemblies = ReflectionHelper.GetAllReferencedAssemblies();
            services.RunModuleInitializers(assemblies);

            // DbContexts
            services.AddAllDbContexts(options =>
            {
                //options.UseStronglyTypeConverters();
                var connectionStrings = configuration.GetValue<string>("DefaultDB:ConnectionStrings");
                ArgumentException.ThrowIfNullOrEmpty(connectionStrings, "DefaultDB:ConnectionStrings");

                options.UseSqlServer(connectionStrings);
            }, assemblies);

            services.AddAuthorization(options =>
            {
                // AddPolicy
                options.AddPolicy(UserRoles.Administrator, policy => policy.RequireRole(UserRoles.Administrator));
            });

            services.AddAuthentication();
            var jwtOpt = configuration.GetSection("JWT").Get<JWTOptions>();
            ArgumentNullException.ThrowIfNull(jwtOpt, "JWT");

            services.AddJWTAuthentication(jwtOpt);
            // 启用 Swagger中的【Authorize】按钮。
            builder.Services.Configure<SwaggerGenOptions>(c =>
            {
                c.AddAuthenticationHeader();
            });

            services.AddMediatR(configuration => configuration.RegisterServicesFromAssemblies(assemblies.ToArray()));
            //不用手动 AddMVC了，因此把文档中的 services.AddMvc(c =>{})改写成Configure<MvcOptions>(c=> {})这个问题很多都类似
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add<UnitOfWorkFilter>();
            });
            //services.Configure<JsonOptions>(options =>
            //{
            //    //设置时间格式。而非“2008-08-08T08:08:08”这样的格式
            //    options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter("yyyy-MM-dd HH:mm:ss"));
            //});

            services.AddCors(options =>
            {
                //更好的在Program.cs中用绑定方式读取配置的方法：https://github.com/dotnet/aspnetcore/issues/21491
                //不过比较麻烦。
                var corsOpt = configuration.GetSection("Cors").Get<CorsSettings>();
                ArgumentNullException.ThrowIfNull(corsOpt, "Cors");

                options.AddDefaultPolicy(builder => builder.WithOrigins(corsOpt.Origins)
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
            var redisConfiguration = configuration.GetValue<string>("Redis:ConnectionStrings");
            ArgumentException.ThrowIfNullOrEmpty(redisConfiguration, "Redis:ConnectionStrings");

            IConnectionMultiplexer redisConnMultiplexer = ConnectionMultiplexer.Connect(redisConfiguration);
            services.AddSingleton(typeof(IConnectionMultiplexer), redisConnMultiplexer);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
            });
        }
    }
}