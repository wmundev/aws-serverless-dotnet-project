using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using OtpNet;
using System.Text.Json;
using JsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace MFAGenerate
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();
        
        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            string secretKeyString = apigProxyEvent.QueryStringParameters["secretKey"];
            byte[] secretKeyBytes = Encoding.UTF8.GetBytes(secretKeyString);
            var totp = new Totp(secretKeyBytes);
            string totpNow = totp.ComputeTotp(DateTime.Now);
            
            var body = new Dictionary<string, string>
            {
                {"totpCode", totpNow}
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