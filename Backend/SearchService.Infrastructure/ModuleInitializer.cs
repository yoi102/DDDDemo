using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SearchService.Domain;
using Zack.Commons;

namespace SearchService.Infrastructure
{
    internal class ModuleInitializer : IModuleInitializer
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddScoped(sp =>
            {
                var option = sp.GetRequiredService<IOptions<ElasticSearchOptions>>();
                var settings = new ElasticsearchClientSettings(option.Value.Url);

                return new ElasticsearchClient(settings);
            });
            services.AddScoped<ISearchRepository, SearchRepository>();
        }
    }
}