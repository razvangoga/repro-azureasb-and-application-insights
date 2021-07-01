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
                //the new SDK does not have the MessageId property set
                dt.Properties.TryGetValue("MessageId", out string messageId);
                Console.WriteLine($"===> Dependency log : {dt.Type}{(string.IsNullOrWhiteSpace(messageId) ? "" : " for message" + messageId)} at: {DateTimeOffset.Now:o}");
            }

            if (item is RequestTelemetry rt)
            {
                //the new SDK does not have the MessageId property set
                rt.Properties.TryGetValue("MessageId", out string messageId);
                Console.WriteLine($"===> Request log : {rt.Name }{(string.IsNullOrWhiteSpace(messageId) ? "" : " for message" + messageId)} at: {DateTimeOffset.Now:o}");
            }
            
            this._next.Process(item);
        }
    }
}