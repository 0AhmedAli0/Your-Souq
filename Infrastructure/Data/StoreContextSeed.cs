using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext _context, UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any(x=> x.UserName == "admin@test.com"))
            {
                var user = new AppUser
                {
                    UserName = "admin@test.com",
                    Email = "admin@test.com"
                };
                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Admin");

            }

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!_context.Products.Any())
            {
                //var JsonProductsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
                var JsonProductsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/products.json");

                var ProductsData = JsonSerializer.Deserialize<List<Product>>(JsonProductsData);

                if (ProductsData == null) return;

                _context.Products.AddRange(ProductsData);
                await _context.SaveChangesAsync();
            }
            if (!_context.DeliveryMethods.Any())
            {
                var JsonMethodsData = await File
                    .ReadAllTextAsync(path + @"/Data/SeedData/delivery.json");
                var MethodsData = JsonSerializer.Deserialize<List<DeliveryMethod>>(JsonMethodsData);

                if (MethodsData == null) return;

                _context.DeliveryMethods.AddRange(MethodsData);
            await _context.SaveChangesAsync();
            }
        }
    }
}
