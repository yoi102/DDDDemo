using Microsoft.AspNetCore.Builder;
using Zack.EventBus;

namespace Initializer
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCommonMiddlewares(this IApplicationBuilder app)
        {
            app.UseEventBus();
            app.UseCors();
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();//不能与 ForwardedHeaders 很好的工作，web api 项目也没必要配置这个
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}