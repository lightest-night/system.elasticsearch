using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace LightestNight.System.ElasticSearch
{
    public interface IElasticSearchClientProvider
    {
        /// <summary>
        /// The ElasticSearch Client
        /// </summary>
        ElasticClient Client { get; }
        
        /// <summary>
        /// The index within ElasticSearch to send entries to by default
        /// </summary>
        string DefaultIndex { get; }

        /// <summary>
        /// Ensures that an index is created in the Search data source for the type given
        /// </summary>
        /// <param name="indexName">The index name to map, default if not given</param>
        /// <param name="customMapping">If any, custom mapping function</param>
        /// <typeparam name="T">The type to map</typeparam>
        /// <exception cref="InvalidOperationException">If any of the ElasticSearch Client requests fail</exception>
        void EnsureIndexWithMapping<T>(string indexName = null, Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> customMapping = null)
            where T : class;

        /// <summary>
        /// Ensures that an index is created in the Search data source for the type given
        /// </summary>
        /// <param name="indexName">The index name to map, default if not given</param>
        /// <param name="customMapping">If any, custom mapping function</param>
        /// <param name="cancellationToken">Any <see cref="CancellationToken" /> to use when ensuring the index is valid</param>
        /// <typeparam name="T">The type to map</typeparam>
        /// <exception cref="InvalidOperationException">If any of the ElasticSearch Client requests fail</exception>
        Task EnsureIndexWithMappingAsync<T>(string indexName = null, Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> customMapping = null, CancellationToken cancellationToken = default)
            where T : class;
    }
}