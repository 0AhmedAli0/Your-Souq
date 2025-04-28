using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController(IGenericRepository<Product> _repository) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Pagination<Product>>>> GetProducts([FromQuery]ProductSpecParameters specParameters)
        {
            var spec = new ProductSpecification(specParameters);
            //var products = await _repository.ListAsync(spec);
            //var count = await _repository.CountAsync(spec);
            //return Ok(new Pagination<Product>(specParameters.pageIndex, specParameters.pageSize, count, products));
            return await CreatePagedResult(_repository, spec, specParameters.pageIndex, specParameters.pageSize);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);
            if(product == null) return NotFound();
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _repository.Add(product);

            if(await _repository.SaveChangesAsync())
            {
                CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            return BadRequest("Probllem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            if (product.Id !=id || !_repository.Exists(id)) 
                return BadRequest("Cannot update this product");

            _repository.Update(product);
            if(await _repository.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem updating the product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null) return NotFound();
            
            _repository.Remove(product);

            if (await _repository.SaveChangesAsync())
            {
                return NoContent();
            }
            return BadRequest("Problem deleting the product");
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            
            var spec = new BrandListSpecification();

            return Ok(await _repository.ListAsync(spec));
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> Gettypes()
        {
            
            var spec = new TypeListSpecification();
            
            return Ok(await _repository.ListAsync(spec));
        }


    }
}
