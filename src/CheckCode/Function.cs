using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using SharedModel;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace CheckCode
{
    public class Function
    {
        private readonly DynamoDBContext _dynamoDbContext;

        public Function()
        {
            var dynamoDbClient = new AmazonDynamoDBClient();
            _dynamoDbContext = new DynamoDBContext(dynamoDbClient);
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            var id = apigProxyEvent.QueryStringParameters["id"];
            var code = Convert.ToInt32(apigProxyEvent.QueryStringParameters["code"]);

            var item = await _dynamoDbContext.LoadAsync<EmailCodeModel>(381435139);

            var responseBody = item.Code == code ? "true" : "false";

            return new APIGatewayProxyResponse
            {
                Body = System.Text.Json.JsonSerializer.Serialize(responseBody),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}