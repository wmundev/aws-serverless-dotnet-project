using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using GenerateSecurePassword;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace HelloWorld.Tests
{
    public class FunctionTest
    {
        private readonly ITestOutputHelper _outputHelper;
        public FunctionTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public async Task InitialisePassword_NoParameter_ReturnsRandomPassword16Char()
        {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();
            request.QueryStringParameters = new Dictionary<string, string>();

            var function = new Function();
            var response = await function.FunctionHandler(request, context);


            var result = System.Text.Json.JsonSerializer.Deserialize<Result>(response.Body);

            Assert.Equal(result.password.Length, 16);
        }

        [Theory]
        [InlineData(16)]
        [InlineData(28)]
        [InlineData(45)]
        public async Task InitialisePassword_Parameter_ReturnsRandomPasswordCharUpToParameter(int length)
        {
            var request = new APIGatewayProxyRequest();
            var context = new TestLambdaContext();
            request.QueryStringParameters = new Dictionary<string, string>();
            request.QueryStringParameters.Add("length", Convert.ToString(length));

            var function = new Function();
            var response = await function.FunctionHandler(request, context);


            var result = System.Text.Json.JsonSerializer.Deserialize<Result>(response.Body);
            _outputHelper.WriteLine(Convert.ToString(length));
            Assert.Equal(result.password.Length, length);
        }
    }

    public class Result
    {
        public string password { get; set; }
    }
}