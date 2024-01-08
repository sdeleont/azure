using System.Threading.Tasks;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;

/*
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
*/
ServiceBusClient client;
ServiceBusProcessor processor;

async Task MessageHandler(ProcessMessageEventArgs args) {
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");
    // New instance of CosmosClient class
    var client = new MongoClient("mongodb://");

    var settings = client.Settings;

    Console.WriteLine(settings.Server.Host);
    // insert one document
    //var myMessage = JsonConvert.DeserializeObject<object>(args.Message.Body.ToString());

    var data1 = new BsonDocument(BsonDocument.Parse(body));
    client.GetDatabase("architectsg-cosmos-db1").GetCollection<BsonDocument>("Weather_Data").InsertOne(data1);
    await args.CompleteMessageAsync(args.Message);
}
Task ErrorHandler(ProcessErrorEventArgs args) { 
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}
var clientOptions = new ServiceBusClientOptions() {
    TransportType = ServiceBusTransportType.AmqpWebSockets
};
client = new ServiceBusClient("Endpoint=sb://" , clientOptions);

processor = client.CreateProcessor("archsq_servicebus_queue", new ServiceBusProcessorOptions());
try {
    processor.ProcessMessageAsync += MessageHandler;
    processor.ProcessErrorAsync+= ErrorHandler;

    await processor.StartProcessingAsync();
    Console.WriteLine("Press ctrl+c to exit");
    await Task.Delay(-1);
    //Console.ReadKey();
    //Console.WriteLine("\nStopping the receiver...");
    //await processor.StopProcessingAsync();
    //Console.WriteLine("Stopped receiving messages");

} finally {
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await processor.StopProcessingAsync();
    await processor.DisposeAsync();
    await client.DisposeAsync();
}