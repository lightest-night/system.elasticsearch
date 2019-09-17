using System;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.Extensions.Options;
using Moq;
using Nest;
using Shouldly;
using Xunit;

namespace LightestNight.System.ElasticSearch.Tests
{
    public class ElasticSearchClientProviderTests
    {
        private const string DefaultIndex = "Test Index";
        private const string Username = "Test";
        private const string Password = "Password";
        
        private readonly Mock<IOptions<ElasticSearchConfig>> _mockConfig = new Mock<IOptions<ElasticSearchConfig>>();
        private IElasticSearchClientProvider _sut;

        public ElasticSearchClientProviderTests()
        {
            _mockConfig.SetupGet(x => x.Value).Returns(new ElasticSearchConfig
            {
                Uri = "http://localhost",
                DefaultIndex = DefaultIndex,
                Username = Username,
                Password = Password
            });
        }

        [Fact]
        public void Should_Create_Valid_ElasticSearchClient_With_BasicAuthentication()
        {
            // Act
            _sut = new ElasticSearchClientProvider(_mockConfig.Object);
            
            // Assert
            _sut.Client.ShouldNotBeNull();
            _sut.Client.ConnectionSettings.DefaultIndex.ShouldBe(DefaultIndex);
            _sut.Client.ConnectionSettings.BasicAuthenticationCredentials.Username.ShouldBe(Username);
            _sut.Client.ConnectionSettings.BasicAuthenticationCredentials.Password.CreateString().ShouldBe(Password);
        }

        [Fact]
        public void Should_Create_Valid_ElasticSearchClient_With_No_Credentials()
        {
            // Arrange
            _mockConfig.SetupGet(x => x.Value).Returns(new ElasticSearchConfig
            {
                Uri = "http://localhost",
                DefaultIndex = DefaultIndex
            });
            
            // Act
            _sut = new ElasticSearchClientProvider(_mockConfig.Object);
            
            // Assert
            _sut.Client.ShouldNotBeNull();
            _sut.Client.ConnectionSettings.DefaultIndex.ShouldBe(DefaultIndex);
            _sut.Client.ConnectionSettings.BasicAuthenticationCredentials.ShouldBeNull();
        }
    }

    public class LogData
    {
        public LogLevel Severity { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
    }

    public class TestConnection
    {
        private readonly ElasticSearchConfig _config = new ElasticSearchConfig
        {
            Uri = "https://ensign-sandbox-2132186282.eu-west-1.bonsaisearch.net:443",
            DefaultIndex = "ensignlogging",
            Username = "gpjxlx5sm6",
            Password = "sudr00ykkz"
        };

        private readonly ElasticSearchClientProvider _sut;

        public TestConnection()
        {
            _sut = new ElasticSearchClientProvider(new OptionsWrapper<ElasticSearchConfig>(_config));
        }

        [Fact]
        public async Task CreateIndex()
        {
            const string indexName = nameof(LogData);
            await _sut.EnsureIndexWithMappingAsync<LogData>(indexName);
            await _sut.Client.IndexDocumentAsync(new LogData
            {
                Severity = LogLevel.Info,
                Message = "Some Message",
                Timestamp = ((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds()
            });
        }
    }
}