using CatalogMicroservice.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogMicroservice.Services.Command
{
    public class CommandService : ICommandService
    {
        DatabaseContext _db;
        public CommandService(DatabaseContext db)
        {
            _db = db;
        }
        public async Task AddProduct(Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateOrder(Product product)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync();
        }
    }
}
