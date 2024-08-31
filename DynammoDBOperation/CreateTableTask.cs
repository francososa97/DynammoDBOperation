using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace DynamoDBOperations
{
    class CreateTableTask
    {
        class TableDefinition
        {
            public string TableName { get; set; }
            public string PartitionKey { get; set; }
            public string SortKey { get; set; }
            public long ReadCapacity { get; set; }
            public long WriteCapacity { get; set; }
        }

        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                IAmazonDynamoDB ddbClient = new AmazonDynamoDBClient();

                var tableDefinition = new TableDefinition
                {
                    TableName = configSettings.TableName,
                    PartitionKey = configSettings.PartitionKey,
                    SortKey = configSettings.SortKey,
                    ReadCapacity = configSettings.ReadCapacity,
                    WriteCapacity = configSettings.WriteCapacity
                };

                Console.WriteLine($"\nCreating an Amazon DynamoDB table \"{configSettings.TableName}\"");
                Console.WriteLine($"with a partition key: {configSettings.PartitionKey}");
                Console.WriteLine($"and sort key: {configSettings.SortKey}.\n");

                var createResponse = await CreateTable(ddbClient, tableDefinition);
                Console.WriteLine($"Table Status: {createResponse.TableDescription.TableStatus}");

                Console.WriteLine("\nWaiting for the table to be available...");
                await WaitForTableCreation(ddbClient, configSettings.TableName);

                Console.WriteLine("\nTable is now available.");
                var tableOutput = await GetTableInfo(ddbClient, configSettings.TableName);
                Console.WriteLine($"Table Status: {tableOutput.Table.TableStatus}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task<CreateTableResponse> CreateTable(IAmazonDynamoDB ddbClient, TableDefinition tableDefinition)
        {
            var request = new CreateTableRequest
            {
                TableName = tableDefinition.TableName,
                ProvisionedThroughput = new ProvisionedThroughput
                {
                    ReadCapacityUnits = tableDefinition.ReadCapacity,
                    WriteCapacityUnits = tableDefinition.WriteCapacity
                },
                KeySchema = new List<KeySchemaElement>
                {
                    new KeySchemaElement
                    {
                        AttributeName = tableDefinition.PartitionKey,
                        KeyType = KeyType.HASH
                    },
                    new KeySchemaElement
                    {
                        AttributeName = tableDefinition.SortKey,
                        KeyType = KeyType.RANGE
                    }
                },
                AttributeDefinitions = new List<AttributeDefinition>
                {
                    new AttributeDefinition
                    {
                        AttributeName = tableDefinition.PartitionKey,
                        AttributeType = "S"
                    },
                    new AttributeDefinition
                    {
                        AttributeName = tableDefinition.SortKey,
                        AttributeType = "N"
                    }
                }
            };

            return await ddbClient.CreateTableAsync(request);
        }

        async Task WaitForTableCreation(IAmazonDynamoDB ddbClient, string tableName)
        {
            var response = await ddbClient.DescribeTableAsync(tableName);
            while (response.Table.TableStatus != TableStatus.ACTIVE)
            {
                Thread.Sleep(500);
                response = await ddbClient.DescribeTableAsync(tableName);
            }
        }

        async Task<DescribeTableResponse> GetTableInfo(IAmazonDynamoDB ddbClient, string tableName)
        {
            return await ddbClient.DescribeTableAsync(tableName);
        }
    }
}
