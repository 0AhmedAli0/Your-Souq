using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CartController(ICartService _cartService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
        {
            var cart = await _cartService.GetCartAsync(id);
            return Ok(cart ?? new ShoppingCart() { Id = id });
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
        {
            var UpdatedCart = await _cartService.SetCartAsync(cart);
            return UpdatedCart!=null ? Ok(UpdatedCart) : BadRequest("problem with cart");
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCart(string id)
        {
            return await _cartService.DeleteCartAsync(id) ? Ok() : BadRequest("proplem deleting cart");
        }
    }
}
