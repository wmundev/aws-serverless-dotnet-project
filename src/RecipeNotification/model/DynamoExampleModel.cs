using Amazon.DynamoDBv2.DataModel;

namespace RecipeNotification.model
{
    [DynamoDBTable("TestTableName")]
    public class DynamoExampleModel
    {
        [DynamoDBHashKey]
        public string Album { get; set; }
        public string Artist { get; set; }
        
    }
}