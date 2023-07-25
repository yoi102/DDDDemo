using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure.EFCore
{
    public static class EFCoreInitializerHelper
    {
        public static IServiceCollection AddAllDbContexts(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction,
            IEnumerable<Assembly> assemblies)
        {

            Type[] types = new Type[] { typeof(IServiceCollection), typeof(Action<DbContextOptionsBuilder>), typeof(ServiceLifetime), typeof(ServiceLifetime) };
            var methodAddDbContext = typeof(EntityFrameworkServiceCollectionExtensions)
                .GetMethod(nameof(EntityFrameworkServiceCollectionExtensions.AddDbContext), 1, types)!;
            foreach (var asmToLoad in assemblies)
            {
                Type[] typesInAsm = asmToLoad.GetTypes();
                foreach (var dbCtxType in typesInAsm
                    .Where(t => !t.IsAbstract && typeof(DbContext).IsAssignableFrom(t)))
                {
                    //similar to serviceCollection.AddDbContextPool<ECDictDbContext>(opt=>new DbContextOptionsBuilder(dbCtxOpt));
                    var methodGenericAddDbContext = methodAddDbContext.MakeGenericMethod(dbCtxType);
                    methodGenericAddDbContext.Invoke(null, new object[] { services, optionsAction, ServiceLifetime.Scoped, ServiceLifetime.Scoped });
                }
            }
            return services;
        }
    }
}