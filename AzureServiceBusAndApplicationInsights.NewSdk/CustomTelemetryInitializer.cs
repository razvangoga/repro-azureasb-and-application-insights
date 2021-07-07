using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureServiceBusAndApplicationInsights.NewSdk
{
    public class CustomTelemetryInitializer: ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry is RequestTelemetry rt && rt.Name == "ServiceBusReceiver.Receive")
            {
                ((ISupportSampling)telemetry).SamplingPercentage = 5;
            }
        }
    }
}