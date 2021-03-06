using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureServiceBusAndApplicationInsights
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISubscriptionClient _subscriptionClient;

        public Worker(ILogger<Worker> logger, ISubscriptionClient subscriptionClient, IConfiguration configuration)
        {
            this._logger = logger;
            this._subscriptionClient = subscriptionClient;
            int maxConcurrentCalls = configuration.GetValue<int>("AzureServiceBus_MaxConcurrentCalls");
            this._subscriptionClient.RegisterMessageHandler(this.MessageHandler, new MessageHandlerOptions(this.MessageErrorHandler)
            {
                MaxConcurrentCalls = maxConcurrentCalls,
                AutoComplete = false
            });
            this._logger.LogInformation($"Starting subscription client with MaxConcurrentCalls={maxConcurrentCalls}{Environment.NewLine}We should see {maxConcurrentCalls} Application Insights asb related dependencies logged every minute");
        }
        
        private async Task MessageHandler(Message message, CancellationToken cancellationToken)
        {
            await this._subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
            this._logger.LogInformation($"Message {message.MessageId} {message.Label} arrived");
        }

        private Task MessageErrorHandler(ExceptionReceivedEventArgs arg)
        {
            this._logger.LogError(arg.Exception,"Message reception failed");
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this._logger.LogInformation($"Worker running at: {DateTimeOffset.Now:O}");
                await Task.Delay(60 * 1000, stoppingToken);
            }
        }
    }
}