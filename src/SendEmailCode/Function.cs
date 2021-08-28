using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using SendEmailCode.dto;
using SendEmailCode.model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace SendEmailCode
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly Random _random = new Random();
        private readonly DynamoDBContext _dynamoDbContext;

        public Function()
        {
            var dynamoDbClient = new AmazonDynamoDBClient();
            _dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            var body = System.Text.Json.JsonSerializer.Deserialize<SendEmailCodeDto>(apigProxyEvent.Body);

            var item = new EmailCodeModel
            {
                Id = _random.Next(),
                Email = body.Email
            };

            await _dynamoDbContext.SaveAsync(item);
            
            return new APIGatewayProxyResponse
            {
                Body = System.Text.Json.JsonSerializer.Serialize(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };
        }
    }
}