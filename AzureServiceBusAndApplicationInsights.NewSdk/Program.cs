using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureServiceBusAndApplicationInsights.NewSdk
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            Activity.ForceDefaultIdFormat = true;
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddLogging();
                    
                    services.AddApplicationInsightsTelemetryWorkerService(o =>
                    {
                        o.EnableAdaptiveSampling = false;
                        o.DeveloperMode = true;
                    });
                    services.AddApplicationInsightsTelemetryProcessor<CustomTelemetryProcessor>();
                    services.AddSingleton<ITelemetryInitializer, CustomTelemetryInitializer>();
                    
                    services.AddSingleton<ServiceBusProcessor>(_ =>
                    {
                        ServiceBusClient client = new ServiceBusClient(hostContext.Configuration.GetValue<string>("AzureServiceBus_ConnectionString"));
                        ServiceBusProcessor processor = client.CreateProcessor(
                            hostContext.Configuration.GetValue<string>("AzureServiceBus_TopicName"), 
                            hostContext.Configuration.GetValue<string>("AzureServiceBus_SubscriptionName"), 
                            new ServiceBusProcessorOptions()
                            {
                                AutoCompleteMessages = false,
                                MaxConcurrentCalls = hostContext.Configuration.GetValue<int>("AzureServiceBus_MaxConcurrentCalls")
                            });
                        return processor;
                    });
                    
                    services.AddHostedService<Worker>();
                })
                .ConfigureLogging((hostContext, builder) =>
                {
                    builder.AddConsole();
                    builder.AddApplicationInsights();
                });

            return host;
        }
    }
}