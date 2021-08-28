using Amazon.DynamoDBv2.DataModel;

namespace SendEmailCode.model
{
    [DynamoDBTable("EmailCode")]
    public class EmailCodeModel
    {
        [DynamoDBHashKey]
        public int Id { get; set; }
        public string Email { get; set; }
    }
}