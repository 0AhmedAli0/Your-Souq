using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext _context)
        {
            if (!_context.Products.Any())
            {
                var JsonProductsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
                var ProductsData = JsonSerializer.Deserialize<List<Product>>(JsonProductsData);

                if (ProductsData == null) return;

                _context.Products.AddRange(ProductsData);
                await _context.SaveChangesAsync();
            }
            if (!_context.DeliveryMethods.Any())
            {
                var JsonMethodsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");
                var MethodsData = JsonSerializer.Deserialize<List<DeliveryMethod>>(JsonMethodsData);

                if (MethodsData == null) return;

                _context.DeliveryMethods.AddRange(MethodsData);
            await _context.SaveChangesAsync();
            }
        }
    }
}
