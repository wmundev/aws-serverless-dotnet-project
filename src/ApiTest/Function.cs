using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace ApiTest
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();
        
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            var body = new Dictionary<string, string>
            {
                {"message", "hello friend"},
            };

            return new APIGatewayProxyResponse
            {
                Body = System.Text.Json.JsonSerializer.Serialize(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
        }
    }
}