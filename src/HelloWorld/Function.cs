using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using JsonSerializer = Amazon.Lambda.Serialization.Json.JsonSerializer;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace HelloWorld
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> GetCallingIP()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var msg = await client.GetStringAsync("http://checkip.amazonaws.com/")
                .ConfigureAwait(continueOnCapturedContext: false);

            return msg.Replace("\n", "");
        }

        private bool CalculatePrimeNumber(int num)
        {
            int m = 0;
            m = num / 2;
            for (int i = 2; i <= m; i++)
            {
                if (num % i == 0)
                {
                    //num is not prime
                    return false;
                }
            }

            return true;
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            string region = Environment.GetEnvironmentVariable("SqsQueueArn");
            context.Logger.LogLine("region" + region);

            int requestBody = int.Parse(apigProxyEvent.Body);

            bool isPrime = CalculatePrimeNumber(requestBody);

            context.Logger.LogLine($"Number {requestBody} prime is {isPrime.ToString()}");

            var location = await GetCallingIP();
            var body = new Dictionary<string, string>
            {
                {"message", "hello world"},
                {"location", location},
                {"isPrimeNumber", isPrime.ToString()}
            };

            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
        }
    }
}