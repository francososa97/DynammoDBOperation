using System;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace DynamoDBOperations
{
    class HighLevelAPITask
    {
        public async Task Run()
        {
            var configSettings = ConfigSettingsReader<DynamoDBConfigSettings>.Read("DynamoDB");

            try
            {
                var ddbClient = new AmazonDynamoDBClient();
                var ddbContext = new DynamoDBContext(ddbClient);

                var userId = configSettings.QueryUserId;
                var noteId = configSettings.QueryNoteId;

                var note = await QuerySpecificNote(ddbContext, userId, noteId);
                var originalNoteText = note.NoteText;
                note.NoteText += ", updated using the object persistence model!";

                await UpdateNote(ddbContext, note);

                note.NoteText = originalNoteText;
                await UpdateNote(ddbContext, note);

                Console.WriteLine("Data query and update using high-level object persistence model completed successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }

        async Task<Note> QuerySpecificNote(DynamoDBContext ddbContext, string userId, int noteId)
        {
            var note = await ddbContext.LoadAsync<Note>(userId, noteId);
            Console.WriteLine($"Returned note object: {note.ToJson()}");
            return note;
        }

        async Task UpdateNote(DynamoDBContext ddbContext, Note note)
        {
            await ddbContext.SaveAsync(note);
            Console.WriteLine("Note updated!");
        }
    }
}
