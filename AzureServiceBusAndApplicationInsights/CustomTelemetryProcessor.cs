using System;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

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
            if (item is DependencyTelemetry dt)
            {
                //the dependencies logged for actual messages have the MessageId property set - a filter may be based on this 
                dt.Properties.TryGetValue("MessageId", out string messageId);
                Console.WriteLine($"===> Dependency log : {dt.Type}{(string.IsNullOrWhiteSpace(messageId) ? "" : " for message" + messageId)} at: {DateTimeOffset.Now:o}");
            }

            if (item is RequestTelemetry rt)
            {
                Console.WriteLine($"===> Request log : {rt.Name } at: {DateTimeOffset.Now:o}");
            }
            
            this._next.Process(item);
        }
    }
}