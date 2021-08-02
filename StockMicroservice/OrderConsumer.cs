using MassTransit;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMicroservice
{
    public class OrderConsumer : IConsumer<IOrder>
    {
        public async Task Consume(ConsumeContext<IOrder> context)
        {
            // TO DO: check stock availibility
            await Console.Out.WriteLineAsync(context.Message.Product);
        }
    }
}
