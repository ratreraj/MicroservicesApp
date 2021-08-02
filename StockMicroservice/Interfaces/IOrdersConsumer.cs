using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockMicroservice.Interfaces
{
    public interface IOrdersConsumer
    {
        void RegisterOnMessageHandlerAndReceiveMessage();
        Task CloseQueueAsync();
    }
}
