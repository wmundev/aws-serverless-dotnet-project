using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using SendEmailCode.dto;
using SharedModel;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(JsonSerializer))]

namespace SendEmailCode
{
    public class Function
    {
        private static readonly HttpClient client = new HttpClient();

        private readonly Random _random = new Random();
        private readonly DynamoDBContext _dynamoDbContext;
        private readonly SmtpClient _client;


        public Function()
        {
            var dynamoDbClient = new AmazonDynamoDBClient();
            _dynamoDbContext = new DynamoDBContext(dynamoDbClient);
            var login = Environment.GetEnvironmentVariable("Login");
            var password = Environment.GetEnvironmentVariable("Password");

            _client = new SmtpClient("email-smtp.us-east-1.amazonaws.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(login, password),
                EnableSsl = true,
            };
        }

        static bool mailSent = false;

        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }

            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }

            mailSent = true;
        }

        public async Task SendEmail(string subject, string body, string receiver)
        {
            var sender = Environment.GetEnvironmentVariable("Sender");

            // Specify the email sender.
            // Create a mailing address that includes a UTF8 character
            // in the display name.
            MailAddress from = new MailAddress(sender, "A Guy", Encoding.UTF8);
            // Set destinations for the email message.
            MailAddress to = new MailAddress(receiver);
            // Specify the message content.
            MailMessage message = new MailMessage(from, to);
            message.Body = body;
            message.Subject = subject;
            // Include some non-ASCII characters in body and subject.
            // string someArrows = new string(new char[] {'\u2190', '\u2191', '\u2192', '\u2193'});
            // message.Body += Environment.NewLine + someArrows;
            message.BodyEncoding = Encoding.UTF8;
            // message.Subject = "test message 1" + someArrows;
            message.SubjectEncoding = Encoding.UTF8;
            // Set the method that is called back when the send operation ends.
            // client.SendCompleted += SendCompletedCallback;
            // The userState can be any object that allows your callback
            // method to identify this send operation.
            // For this example, the userToken is a string constant.
            // string userState = "test message1";
            // client.SendAsync(message, userState);
            await _client.SendMailAsync(message);

            // Clean up.
            message.Dispose();
        }

        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent,
            ILambdaContext context)
        {
            var body = System.Text.Json.JsonSerializer.Deserialize<SendEmailCodeDto>(apigProxyEvent.Body);
            var receiver = body.Email;

            var CodeGenerated = _random.Next(10000, 99999);

            var item = new EmailCodeModel
            {
                Id = _random.Next(),
                Email = receiver,
                Code = CodeGenerated
            };

            await _dynamoDbContext.SaveAsync(item);

            await SendEmail("Your security code", $"Your code is {CodeGenerated}", receiver);

            return new APIGatewayProxyResponse
            {
                Body = System.Text.Json.JsonSerializer.Serialize(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }
}