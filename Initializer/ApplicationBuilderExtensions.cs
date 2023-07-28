using Microsoft.AspNetCore.Builder;
using Zack.EventBus;

namespace Initializer
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDefaultMiddleware(this IApplicationBuilder app)
        {
            app.UseEventBus();
            app.UseCors();
            app.UseForwardedHeaders();
            app.UseHttpsRedirection();//不能与ForwardedHeaders很好的工作，webapi项目也没必要配置这个
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}