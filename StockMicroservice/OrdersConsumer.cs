using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using SharedModels;
using StockMicroservice.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StockMicroservice
{
    public class OrdersConsumer : IOrdersConsumer
    {
        QueueClient _queueClient;
        IConfiguration _config;
        public OrdersConsumer(IConfiguration config)
        {
            _config = config;
            _queueClient = new QueueClient(_config["AzureServiceBus:Connection"], _config["AzureServiceBus:Queue"]);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            var order =JsonSerializer.Deserialize<IOrder>(Encoding.UTF8.GetString(message.Body));
            
            // TO DO: check stock availibility
            await Console.Out.WriteLineAsync(order.Product);

            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
         
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            return Task.CompletedTask;
        }


        public async Task CloseQueueAsync()
        {
            await _queueClient.CloseAsync();
        }

        public void RegisterOnMessageHandlerAndReceiveMessage()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }
    }
}
