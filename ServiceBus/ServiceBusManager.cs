using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using System.Text.Json;

namespace ResponsibilityHub.ServiceBus;
public class ServiceBusManager : IHostedService
{
    private const string qName = "sebdzi-q";
    private readonly ServiceBusClient sbClient;
    private readonly ServiceBusProcessor processor;
    public ServiceBusManager(IAzureClientFactory<ServiceBusClient> clientFactory)
    {
        sbClient = clientFactory.CreateClient("sbClient");
        processor = sbClient.CreateProcessor(qName);
        processor.ProcessMessageAsync += Processor_ProcessMassageAsync;
        processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
    }
    private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        throw new NotImplementedException();
    }
    private async Task Processor_ProcessMassageAsync(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        var ev = JsonSerializer.Deserialize<EventGridModel>(body, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        var data = ev.Data;
        var url = data.Url;
        Console.WriteLine(url);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await processor.StartProcessingAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await processor.StopProcessingAsync();
    }
}
