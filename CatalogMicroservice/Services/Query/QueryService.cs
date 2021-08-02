using CatalogMicroservice.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogMicroservice.Services.Query
{
    public class QueryService : IQueryService
    {
        DatabaseContext _db;
        public QueryService(DatabaseContext db)
        {
            _db = db;
        }
        public async Task<Product> GetProductById(int Id)
        {
            return await _db.Products.Where(p => p.ProductId == Id).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _db.Products.AsNoTracking().ToListAsync();
        }
    }
}
