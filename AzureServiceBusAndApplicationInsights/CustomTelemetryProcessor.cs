using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;

namespace AzureServiceBusAndApplicationInsights
{
    public class CustomTelemetryProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public CustomTelemetryProcessor(ITelemetryProcessor next)
        {
            this._next = next;
        }
        
        public void Process(ITelemetry item)
        {
            if (item is DependencyTelemetry telemetry)
            {
                Console.WriteLine($"===> Dependency log : {telemetry.Type} dependency at: {DateTimeOffset.Now:o}");
            }
            
            this._next.Process(item);
        }
    }
}