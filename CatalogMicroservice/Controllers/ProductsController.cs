using CatalogMicroservice.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        DatabaseContext _db;
        public ProductController(DatabaseContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>),StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Get Products List", OperationId = "GetProducts")]
        public IEnumerable<Product> GetProducts()
        {
            return _db.Products.ToList();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(Summary ="Add Product", OperationId = "AddProduct")]
        public IActionResult AddProduct(Product model)
        {
            try
            {
                _db.Products.Add(model);
                _db.SaveChanges();
                return StatusCode(StatusCodes.Status201Created);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
