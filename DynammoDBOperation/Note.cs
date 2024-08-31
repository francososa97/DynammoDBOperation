using System;
using System.Text.Json;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBOperations
{
    [DynamoDBTable("Notes")]
    class Note
    {
        [DynamoDBHashKey]
        public string UserId { get; set; }

        [DynamoDBRangeKey]
        public int NoteId { get; set; }

        [DynamoDBProperty("Note")]
        public string NoteText { get; set; }

        [DynamoDBIgnore]
        public string SomeOtherData { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
