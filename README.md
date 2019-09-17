# Lightest Night
## ElasticSearch

Helper functions and utilities to access an instance of ElasticSearch

#### How To Use
##### Registration
* Asp.Net Standard/Core Dependency Injection
  * Use the provided `services.AddElasticSearch()` method
  
* Other Containers
  * Configure the ElasticSearchConfig object as Options
  * Register an instance of `ElasticSearchClientProvider` as the `IElasticSearchClientProvider` type with a Transient Lifecycle

##### Usage
###### Synchronous
Call `EnsureIndexWithMapping<T>(string indexName = null, Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> customMapping = null)`

###### Asynchronous
Call `EnsureIndexWithMappingAsync<T>(string indexName = null, Func<PutMappingDescriptor<T>, PutMappingDescriptor<T>> customMapping = null)`
