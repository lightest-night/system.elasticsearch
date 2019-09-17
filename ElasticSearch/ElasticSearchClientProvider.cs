using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Nest;

namespace LightestNight.System.ElasticSearch
{
    public class ElasticSearchClientProvider : IElasticSearchClientProvider
    {
        /// <inheritdoc cref="IElasticSearchClientProvider.Client" />
        public ElasticClient Client { get; }
        
        /// <inheritdoc cref="IElasticSearchClientProvider.DefaultIndex" />
        public string DefaultIndex { get; }

        public ElasticSearchClientProvider(IOptions<ElasticSearchConfig> options)
        {
            var config = options.Value;
            var settings = new ConnectionSettings(new Uri(config.Uri))
                .DefaultIndex(config.DefaultIndex);

            if (!string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password))
                settings.BasicAuthentication(config.Username, config.Password);

            settings.DisableDirectStreaming()
                .PrettyJson()
                .OnRequestCompleted(response =>
                {
                    // Log out the request
                    Debug.WriteLine(response.RequestBodyInBytes != null
                        ? $"{response.HttpMethod} {response.Uri}{Environment.NewLine}{Encoding.UTF8.GetString(response.RequestBodyInBytes)}"
                        : $"{response.HttpMethod} {response.Uri}");

                    Debug.WriteLine(null);

                    // Log out the response
                    Debug.WriteLine(response.ResponseBodyInBytes != null
                        ? $"Status: {response.HttpStatusCode}{Environment.NewLine}{Encoding.UTF8.GetString(response.ResponseBodyInBytes)}{Environment.NewLine}{new string('-', 30)}{Environment.NewLine}"
                        : $"Status: {response.HttpStatusCode}{Environment.NewLine}{new string('-', 30)}");
                });
            
            Client = new ElasticClient(settings);
            DefaultIndex = config.DefaultIndex;
        }

        /// <inheritdoc cref="IElasticSearchClientProvider.EnsureIndexWithMapping{T}" />
        public void EnsureIndexWithMapping<T>(string indexName = null, Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> customMapping = null)
            where T : class
        {
            if (string.IsNullOrEmpty(indexName))
                indexName = DefaultIndex;
            indexName = indexName.ToLower();
            
            // Map type T to the index T represents
            Client.ConnectionSettings.DefaultIndices.Add(typeof(T), indexName);
            
            // Check if the index already exists
            var indexExistsResponse = Client.Indices.Exists(new IndexExistsRequest(indexName));
            if (!indexExistsResponse.IsValid && indexExistsResponse.ServerError != null)
                throw new InvalidOperationException(indexExistsResponse.DebugInformation, indexExistsResponse.OriginalException);
            
            // If it exists, we're done
            if (indexExistsResponse.Exists)
                return;
            
            // Otherwise, create the index and type mapping
            var createIndexResult = Client.Indices.Create(indexName);
            if (!createIndexResult.IsValid)
                throw new InvalidOperationException(createIndexResult.DebugInformation, createIndexResult.OriginalException);

            var result = Client.Map<T>(mapping =>
            {
                mapping.AutoMap().Index(indexName);
                if (customMapping != null)
                    mapping = customMapping(mapping);

                return mapping;
            });
            
            if (!result.IsValid)
                throw new InvalidOperationException(result.DebugInformation, result.OriginalException);
        }

        /// <inheritdoc cref="IElasticSearchClientProvider.EnsureIndexWithMappingAsync{T}" />
        public async Task EnsureIndexWithMappingAsync<T>(string indexName = null, Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> customMapping = null, CancellationToken cancellationToken = default)
            where T : class
        {
            if (string.IsNullOrEmpty(indexName))
                indexName = DefaultIndex;
            indexName = indexName.ToLower();
            
            // Map type T to the index T represents
            Client.ConnectionSettings.DefaultIndices.Add(typeof(T), indexName);
            
            // Check if the index already exists
            var indexExistsResponse = await Client.Indices.ExistsAsync(new IndexExistsRequest(indexName), cancellationToken);
            if (!indexExistsResponse.IsValid && indexExistsResponse.ServerError != null)
                throw new InvalidOperationException(indexExistsResponse.DebugInformation, indexExistsResponse.OriginalException);
            
            // If it exists, we're done
            if (indexExistsResponse.Exists)
                return;
            
            // Otherwise, create the index and type mapping
            var createIndexResult = await Client.Indices.CreateAsync(indexName, ct: cancellationToken);
            if (!createIndexResult.IsValid)
                throw new InvalidOperationException(createIndexResult.DebugInformation, createIndexResult.OriginalException);

            var result = await Client.MapAsync<T>(mapping =>
            {
                mapping.AutoMap().Index(indexName);
                if (customMapping != null)
                    mapping = customMapping(mapping);

                return mapping;
            }, cancellationToken);
            
            if (!result.IsValid)
                throw new InvalidOperationException(result.DebugInformation, result.OriginalException);
        }
    }
}