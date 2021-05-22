using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureServiceBusAndApplicationInsights
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
                    
                    services.AddSingleton<ISubscriptionClient>(new SubscriptionClient(hostContext.Configuration.GetValue<string>("AzureASBConnString"), "watch-for-items", "watcher1"));
                    
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