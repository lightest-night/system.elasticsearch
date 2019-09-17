using System;
using LightestNight.System.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LightestNight.System.ElasticSearch
{
    public static class ExtendsServiceCollection
    {
        public static IServiceCollection AddElasticSearch(this IServiceCollection services, Action<ElasticSearchConfig> elasticSearchOptions = null)
        {
            if (elasticSearchOptions == null)
            {
                services.AddConfiguration();
                var configurationRoot = services.BuildServiceProvider().GetRequiredService<ConfigurationManager>().Configuration;

                services.Configure<ElasticSearchConfig>(configurationRoot);
            }
            else
            {
                services.Configure(elasticSearchOptions);
            }

            return services.AddTransient(typeof(IElasticSearchClientProvider), typeof(ElasticSearchClientProvider));
        }
    }
}