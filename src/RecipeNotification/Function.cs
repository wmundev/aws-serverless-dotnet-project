using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.S3;
using Nest;
using RecipeNotification.model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace RecipeNotification
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();
        
        public async Task FunctionHandler(ScheduledEvent scheduledEvent,
            ILambdaContext context)
        {
            var elasticsearchDomain = Environment.GetEnvironmentVariable("ElasticSearchEndpoint");
            var esDomain = $"https://{elasticsearchDomain}";
            
            var settings = new ConnectionSettings(new Uri(esDomain));
            var elasticClient = new ElasticClient(settings);
            Guid guid = Guid.NewGuid();
            var elasticBody = new Entry
            {
                Id = guid.ToString(),
                FirstName="wowo"
            };
            var response = await elasticClient.IndexAsync(elasticBody, i => i.Index("people"));
            Console.WriteLine(response.IsValid);
            Console.WriteLine(response.DebugInformation);
        }
    }
}