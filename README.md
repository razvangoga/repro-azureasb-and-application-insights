# Scenario

Sandbox repro for a netcore 3.1 worker service that listens for messages from a Azure Service Bus subscription, while tracking telemetry with Azure Application Insights.

The ASB SDK publishes once every minute a _heart-beat_ in Application Insights as a **Dependency** (for Microsoft.Azure.ServiceBus v5.1.3) or **Request** (Azure.Messaging.ServiceBus v7.1.2).

If the subscription client's **MaxConcurrentCalls** is set above 1, the SDK _seems to publish **the number set to MaxConcurrentCalls** dependency hart-beats every minute_ 

# How to run

In **appsettings.json**
- add your Application Insights instrumentation key
- add an ASB connection string, topic name and subscription name

Run one of the projects (requires netcore 3.1)
1. **AzureServiceBusAndApplicationInsights** uses the old style SDK **Microsoft.Azure.ServiceBus v5.1.3**
2. **AzureServiceBusAndApplicationInsights.NewSdk** uses the new style SDK **Azure.Messaging.ServiceBus v7.1.2**

