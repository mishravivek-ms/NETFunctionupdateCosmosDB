using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.CosmosDB;
using Azure.Messaging.EventHubs;
using Microsoft.Extensions.Logging;
using System.Configuration;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;
using System.Data;

namespace latestEventHubNET
{
    public static class Function4
    {
        [FunctionName("Function4")]
        public static async Task Run(
            [EventHubTrigger("fortheventhub", Connection = "eventhubconnection")] EventData[] events,
            [CosmosDB(
    databaseName: "cosmosDBOutput",
    collectionName: "demoitems",
    ConnectionStringSetting = "Cosmos_DB_Connection_String",
    CreateIfNotExists = true)] IAsyncCollector<dynamic> cosmosDBOutput, ILogger log)
        {
            var exceptions = new List<Exception>();
            dynamic output = null;

            foreach (EventData eventData in events)
            {
                try
                {
                    // Replace these two lines with your processing logic.
                    log.LogInformation($"C# Event Hub trigger function processed a message: {eventData.EventBody}");
                    output = await ProcessEvent(eventData, cosmosDBOutput);
                    //invoke InvokeStoredProcedure method
                    await InvokeStoredProcedure(eventData.EventBody.ToString());
                    //print success message
                    log.LogInformation($"C# SUCCESS---------->: {output}");
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.
            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
        }

        private static async Task<List<dynamic>> ProcessEvent(EventData eventData, IAsyncCollector<dynamic> cosmosDBOutput)
        {
            string messageBody = Encoding.UTF8.GetString(eventData.EventBody.ToArray());

            // Convert the message body to a list of dynamic objects
            List<dynamic> result = JsonConvert.DeserializeObject<List<dynamic>>(messageBody);

            // Add each item in the result to the CosmosDB output
            foreach (var item in result)
            {
                await cosmosDBOutput.AddAsync(item);
            }

            return result;
        }

        public static async Task InvokeStoredProcedure(string connectionString)
        {
            string sqlServerConnectionString = Environment.GetEnvironmentVariable("SqlServerConnectionString");
            using (SqlConnection connection = new SqlConnection(sqlServerConnectionString))
            {
                using (SqlCommand command = new SqlCommand("InsertEmployee", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    Random random = new Random();
                    int randomId = random.Next(301, 400);
                    // Replace the hardcoded value with the value from the EventData object
                    command.Parameters.Add(new SqlParameter("@ID", "4" + randomId));
                    command.Parameters.Add(new SqlParameter("@FirstName", "Hello4"));
                    command.Parameters.Add(new SqlParameter("@LastName", "World"));

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
