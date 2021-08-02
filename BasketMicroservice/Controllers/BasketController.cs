using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetData(int id)
        {
            // TO DO:
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
