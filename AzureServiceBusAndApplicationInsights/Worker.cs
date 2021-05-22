using System;
using System.Collections.Generic;
using System.Linq;
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
        
        private Task MessageHandler(Message arg1, CancellationToken arg2)
        {
            this._logger.LogInformation($"Message {arg1.MessageId} {arg1.Label} arrived");
            return Task.CompletedTask;
        }

        private Task MessageErrorHandler(ExceptionReceivedEventArgs arg)
        {
            this._logger.LogInformation($"Message reception failed : {arg.Exception}");
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