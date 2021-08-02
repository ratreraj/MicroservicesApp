using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderMicroservice.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IPublishEndpoint _publishEndpoint;
        private IHttpClientFactory _clientFactory;
        public OrderController(IPublishEndpoint publishEndpoint, IHttpClientFactory clientFactory)
        {
            _publishEndpoint = publishEndpoint;
            _clientFactory = clientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] IOrder order)
        {
            order.OrderId = Guid.NewGuid();
            await _publishEndpoint.Publish<IOrder>(order);
            return Ok();
        }

        [HttpPost]
        public IActionResult PlaceOrder(int Id)
        {
            try
            {
                var client = _clientFactory.CreateClient("basketmicroservice");
                var response = client.GetAsync("https://localhost:44309/api" + "/Basket/" + Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    // TO DO:
                }
                else
                {
                    return StatusCode((int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                //var data = JsonSerializer.Serialize(ex);
                var data = JsonConvert.SerializeObject(ex);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");
                var client = _clientFactory.CreateClient("logmicroservice");
                _ = client.PostAsync("https://localhost:44380/api" + "/Log/", content).Result;

                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
            return Ok();
        }
    }
}
