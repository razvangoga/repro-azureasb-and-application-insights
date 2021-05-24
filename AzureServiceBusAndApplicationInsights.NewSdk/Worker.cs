using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AzureServiceBusAndApplicationInsights.NewSdk
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ServiceBusProcessor _processor;

        public Worker(ILogger<Worker> logger, IConfiguration configuration, ServiceBusProcessor processor)
        {
            this._logger = logger;
            this._processor = processor;
            this._processor.ProcessMessageAsync += ProcessorOnProcessMessageAsync;
            this._processor.ProcessErrorAsync += ProcessorOnProcessErrorAsync;
            
            int maxConcurrentCalls = configuration.GetValue<int>("AzureServiceBus_MaxConcurrentCalls");
            this._logger.LogInformation($"Starting subscription client with MaxConcurrentCalls={maxConcurrentCalls}{Environment.NewLine}We should see {maxConcurrentCalls} Application Insights asb related dependencies logged every minute");
        }

        private Task ProcessorOnProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            this._logger.LogError(arg.Exception, "message handling failed");
            return Task.CompletedTask;
        }

        private async Task ProcessorOnProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            await arg.CompleteMessageAsync(arg.Message);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await this._processor.StartProcessingAsync(cancellationToken);
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