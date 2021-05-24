using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureServiceBusAndApplicationInsights.NewSdk
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
            if (item is DependencyTelemetry dt)
            {
                Console.WriteLine($"===> Dependency log : {dt.Type} at: {DateTimeOffset.Now:o}");
            }

            if (item is RequestTelemetry rt)
            {
                Console.WriteLine($"===> Request log : {rt.Name } at: {DateTimeOffset.Now:o}");
            }
            
            this._next.Process(item);
        }
    }
}