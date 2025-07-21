using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class PaymentsController(IPaymentService paymentService,
        IUnitOfWork unitOfWork) : BaseApiController
    {
        [Authorize]
        [HttpPost("{cardId}")]
        public async Task<ActionResult<ShoppingCart>> CreateOrUpdatePaymentIntent(string cardId)
        {
            var cart = await paymentService.CreateOrUpdatePaymentIntent(cardId);

            if (cart == null) return BadRequest("Problem with your cart");

            return Ok(cart);
        }

        [HttpGet("delivery-methods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            return Ok(await unitOfWork.Repository<DeliveryMethod>().GetAllAsync());
        } 
    }
}
