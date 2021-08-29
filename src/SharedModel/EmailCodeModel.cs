using Amazon.DynamoDBv2.DataModel;

namespace SharedModel
{
    [DynamoDBTable("EmailCode")]
    public class EmailCodeModel
    {
        [DynamoDBHashKey]
        public int Id { get; set; }
        public string Email { get; set; }
        public int Code { get; set; }
    }
}