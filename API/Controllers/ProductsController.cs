using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IProductRepository _repository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand, string? type, string? sort)
        {
            return Ok(await _repository.GetProductsAsync(brand, type, sort));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);
            if(product == null) return NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _repository.AddProduct(product);

            if(await _repository.SaveChangesAsync())
            {
                CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            return BadRequest("Probllem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            if (product.Id !=id || !_repository.ProductExists(id)) 
                return BadRequest("Cannot update this product");

            _repository.UpdateProduct(product);
            if(await _repository.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem updating the product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetProductByIdAsync(id);

            if (product == null) return NotFound();
            
            _repository.DeleteProduct(product);

            if (await _repository.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem deleting the product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await _repository.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> Gettypes()
        {
            return Ok(await _repository.GetTypesAsync());
        }

        //private bool ProductExists(int id)
        //{
        //    return _repository.ProductExists(id);
        //}
    }
}
