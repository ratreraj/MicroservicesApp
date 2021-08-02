using CatalogMicroservice.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogMicroservice.Services.Command
{
    public interface ICommandService
    {
        Task AddProduct(Product product);
        Task UpdateOrder(Product product);
    }
}
