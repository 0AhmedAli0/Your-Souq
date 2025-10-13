using API.RequestHelpers;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController(IUnitOfWork unitOfWork) : BaseApiController
    {
        [Cache(600)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Pagination<Product>>>> GetProducts([FromQuery]ProductSpecParameters specParameters)
        {
            var spec = new ProductSpecification(specParameters);
            //var products = await _repository.ListAsync(spec);
            //var count = await _repository.CountAsync(spec);
            //return Ok(new Pagination<Product>(specParameters.pageIndex, specParameters.pageSize, count, products));
            return await CreatePagedResult(unitOfWork.Repository<Product>(), spec, specParameters.pageIndex, specParameters.pageSize);
        }

        [Cache(600)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);
            if(product == null) return NotFound();
            return product;
        }

        [InvalidateCache("api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            unitOfWork.Repository<Product>().Add(product);

            if(await unitOfWork.Complete())
            {
               return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }

            return BadRequest("Probllem creating product");
        }

        [InvalidateCache("api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            if (product.Id !=id || !unitOfWork.Repository<Product>().Exists(id)) 
                return BadRequest("Cannot update this product");

            unitOfWork.Repository<Product>().Update(product);
            if(await unitOfWork.Complete())
            {
                return NoContent();
            }
            return BadRequest("Problem updating the product");
        }

        [InvalidateCache("api/products|")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null) return NotFound();
            
            unitOfWork.Repository<Product>().Remove(product);

            if (await unitOfWork.Complete())
            {
                return NoContent();
            }
            return BadRequest("Problem deleting the product");
        }

        [Cache(10000)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            
            var spec = new BrandListSpecification();

            return Ok(await unitOfWork.Repository<Product>().ListAsync(spec));
        }

        [Cache(10000)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> Gettypes()
        {
            
            var spec = new TypeListSpecification();
            
            return Ok(await unitOfWork.Repository<Product>().ListAsync(spec));
        }


    }
}
