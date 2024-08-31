using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBOperations
{
    class QueryDataTask
    {
        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                var ddbClient = new AmazonDynamoDBClient();
                var notes = await QueryNotesByPartitionKey(ddbClient, configSettings.TableName, configSettings.QueryUserId);
                Print(notes);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task<IEnumerable<Dictionary<string, AttributeValue>>> QueryNotesByPartitionKey(IAmazonDynamoDB ddbClient, string tableName, string userId)
        {
            var request = new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "UserId = :userId",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":userId", new AttributeValue(userId) }
                },
                ProjectionExpression = "NoteId, Note"
            };

            var response = await ddbClient.QueryAsync(request);
            return response.Items;
        }

        void Print(IEnumerable<Dictionary<string, AttributeValue>> notes)
        {
            foreach (var note in notes)
            {
                var json = JsonSerializer.Serialize(new
                {
                    NoteId = note["NoteId"].N.ToString(),
                    Note = note["Note"].S
                });

                Console.WriteLine(json);
            }
        }
    }
}
