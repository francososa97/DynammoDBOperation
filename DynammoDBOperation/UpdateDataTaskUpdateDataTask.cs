using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBOperations
{
    class UpdateDataTask
    {
        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                var ddbClient = new AmazonDynamoDBClient();
                await UpdateNewAttribute(ddbClient, configSettings.TableName, configSettings.QueryUserId, configSettings.QueryNoteId);
                await UpdateExistingAttributeConditionally(ddbClient, configSettings.TableName, configSettings.QueryUserId, configSettings.QueryNoteId, configSettings.NotePrefix);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task UpdateNewAttribute(IAmazonDynamoDB ddbClient, string tableName, string userId, int noteId)
        {
            // Implementación de la lógica para actualizar el atributo nuevo.
        }

        async Task UpdateExistingAttributeConditionally(IAmazonDynamoDB ddbClient, string tableName, string userId, int noteId, string notePrefix)
        {
            try
            {
                var request = new UpdateItemRequest
                {
                    TableName = tableName,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "UserId", new AttributeValue(userId) },
                        { "NoteId", new AttributeValue { N = noteId.ToString() } }
                    },
                    ReturnValues = ReturnValue.ALL_NEW,
                    UpdateExpression = "SET Note = :NewNote, Is_Incomplete = :new_incomplete",
                    ConditionExpression = "Is_Incomplete = :old_incomplete",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":NewNote", new AttributeValue($"{notePrefix} 0 KB") },
                        { ":new_incomplete", new AttributeValue("No") },
                        { ":old_incomplete", new AttributeValue("Yes") }
                    }
                };

                var response = await ddbClient.UpdateItemAsync(request);
                Print(response.Attributes);
            }
            catch (ConditionalCheckFailedException)
            {
                Console.WriteLine("Update invalid!");
            }
        }

        void Print(IDictionary<string, AttributeValue> attributes)
        {
            var json = JsonSerializer.Serialize(new
            {
                UserId = attributes["UserId"].S,
                NoteId = attributes["NoteId"].N,
                Note = attributes["Note"].S,
                Is_Incomplete = attributes["Is_Incomplete"].S
            });

            Console.WriteLine(json);
        }
    }
}
