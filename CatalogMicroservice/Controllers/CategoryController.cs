using CatalogMicroservice.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        DatabaseContext _db;
        public CategoryController(DatabaseContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IEnumerable<Category> GetCategories()
        {
            return _db.Categories.ToList();
        }

        [HttpPost]
        public IActionResult AddCategory(Category model)
        {
            try
            {
                _db.Categories.Add(model);
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
