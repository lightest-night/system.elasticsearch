using System;

namespace LightestNight.System.ElasticSearch
{
    public class ElasticSearchConfig
    {
        /// <summary>
        /// The Uri to find the ElasticSearch instance
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// The name of the index to use in absence of one being provided
        /// </summary>
        public string DefaultIndex { get; set; }
        
        /// <summary>
        /// The Username needed to connect to the ElasticSearch instance
        /// </summary>
        public string Username { get; set; }
        
        /// <summary>
        /// The Password needed to connect to the ElasticSearch instance
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// If necessary to build out a region, it goes here
        /// </summary>
        /// <remarks>Useful for cloud providers such as AWS</remarks>
        public string Region { get; set; } = Environment.GetEnvironmentVariable("REGION");
    }
}