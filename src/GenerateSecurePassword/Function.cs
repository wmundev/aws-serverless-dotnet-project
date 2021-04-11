using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using PasswordGenerator;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace GenerateSecurePassword
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();

        private IPassword initialisePassword(string length = "16")
        {
            Console.WriteLine($"hi {length}");
            return new Password(Convert.ToInt32(length));
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            string key = "length";
            var passwordLength = apigProxyEvent.QueryStringParameters.ContainsKey(key)
                ? apigProxyEvent.QueryStringParameters[key]
                : String.Empty;
            IPassword passwordClass = passwordLength == String.Empty
                ? initialisePassword()
                : initialisePassword(passwordLength);
            string password = passwordClass.Next();

            var body = new Dictionary<string, string>
            {
                {"password", password}
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