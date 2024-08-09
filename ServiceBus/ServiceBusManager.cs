using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Azure;
using System.Diagnostics;
using System.Text.Json;

namespace ResponsibilityHub.ServiceBus;
public class ServiceBusManager : IHostedService
{
    private readonly ServiceBusClient serviceBusClient;
    private readonly ServiceBusProcessor processor;
    private readonly HttpClient httpClient;
    private readonly ServiceBusConfiguration configuration;

    public ServiceBusManager(IAzureClientFactory<ServiceBusClient> clientFactory,
                                 ServiceBusConfiguration configuration,
                                 IHttpClientFactory httpClientFactory)
    {
        this.configuration = configuration;
        httpClient = httpClientFactory.CreateClient();
        serviceBusClient = clientFactory.CreateClient(configuration.Name);
        processor = serviceBusClient.CreateProcessor(configuration.QueueName);
        processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
        processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
    }
    private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs args)
    {
        Debug.WriteLine($"Error processing message: {args.Exception}");
        return Task.CompletedTask;
        //throw new NotImplementedException();
    }
    private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            string body = args.Message.Body.ToString();
            var ev = JsonSerializer.Deserialize<EventGridModel>(body, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
            var data = ev.Data;
            var url = data.Url;
            Console.WriteLine(url);

            var response = await httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Received data: " + responseData);
            }
            else
            {
                Console.WriteLine($"Failed to fetch data from {url}: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Exception occurred while processing message: {ex.Message}");
        }
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await processor.StartProcessingAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await processor.StopProcessingAsync(cancellationToken);
        await processor.DisposeAsync();
    }
}
