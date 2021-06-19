using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace HelloWorld.Tests
{
    public class FunctionTest
    {
        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> GetCallingIP()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var stringTask = client.GetStringAsync("http://checkip.amazonaws.com/")
                .ConfigureAwait(continueOnCapturedContext: false);

            var msg = await stringTask;
            return msg.Replace("\n", "");
        }

        [Fact]
        public async Task TestHelloWorldFunctionHandler()
        {
            var request = new APIGatewayProxyRequest();
            request.Body = "3";

            var context = new TestLambdaContext();
            string location = GetCallingIP().Result;
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                {"message", "hello world"},
                {"location", location},
            };

            var expectedResponse = new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };

            var function = new Function();
            var response = await function.FunctionHandler(request, context);

            Console.WriteLine("Lambda Response: \n" + response.Body);
            Console.WriteLine("Expected Response: \n" + expectedResponse.Body);

            Assert.Equal(expectedResponse.Body, response.Body);
            Assert.Equal(expectedResponse.Headers, response.Headers);
            Assert.Equal(expectedResponse.StatusCode, response.StatusCode);
        }

        [Fact]
        public async Task FunctionHandler_Called_LogLineHello()
        {
            // Arrange
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();
            string location = GetCallingIP().Result;
            Dictionary<string, string> body = new Dictionary<string, string>
            {
                {"message", "hello world"},
                {"location", location},
            };

            var expectedResponse = new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> {{"Content-Type", "application/json"}}
            };

            var contextMock = new Mock<ILambdaLogger>();
            context.Logger = contextMock.Object;
            // Act
            var function = new Function();
            var response = await function.FunctionHandler(request, context);

            // Assert
            contextMock.Verify(logger => logger.LogLine("hello!"));
        }
    }
}