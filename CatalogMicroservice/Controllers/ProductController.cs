using CatalogMicroservice.Database;
using CatalogMicroservice.Services.Command;
using CatalogMicroservice.Services.Query;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CatalogMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        ICommandService _commandService;
        IQueryService _queryService;
        public ProductsController(ICommandService commandService,
        IQueryService queryService)
        {
            _commandService = commandService;
            _queryService = queryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get Products List", OperationId = "GetProducts")]
        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _queryService.GetProducts();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary = "Add Product", OperationId = "AddProduct")]
        public async Task<IActionResult> AddProduct(Product model)
        {
            try
            {
                await _commandService.AddProduct(model);
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
