using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using SharedModels;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        QueueClient _queueClient;
        IConfiguration _config;
        public OrdersController(IConfiguration config)
        {
            _config = config;
            _queueClient = new QueueClient(_config["AzureServiceBus:Connection"], _config["AzureServiceBus:Queue"]);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] IOrder order)
        {
            order.OrderId = Guid.NewGuid();
            string data = JsonSerializer.Serialize(order);
            Message msg = new Message(Encoding.UTF8.GetBytes(data));
            await _queueClient.SendAsync(msg);
            return Ok();
        }
    }
}
