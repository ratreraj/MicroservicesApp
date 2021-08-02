using CatalogMicroservice.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogMicroservice.Services.Query
{
    public interface IQueryService
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetProductById(int Id);
    }
}
