using System.Security.Cryptography;
using API.Dtos;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IUnitOfWork unitOfWork, IPaymentService paymentService) : BaseApiController
    {
        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders([FromQuery]OrderSpecParameters specParameters)
        {
            var spec = new OrderSpecification(specParameters);
            return await CreatePagedResult(unitOfWork.Repository<Order>(), spec, specParameters.pageIndex, specParameters.pageSize, O => O.ToDto() );

        }

        [HttpGet("orders/{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var spec = new OrderSpecification(id);
            var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return BadRequest("No order with that id");
            return order.ToDto();
        }

        [HttpPost("orders/refund/{id:int}")]
        public async Task<ActionResult<OrderDto>> RefundOrder(int id)
        {
            var spec = new OrderSpecification(id);
            var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return BadRequest("No order with that id");
            if (order.Status != OrderStatus.Pending) return BadRequest("payment not received for this order");

            var result = await paymentService.RefundPayment(order.PaymentIntentId);

            if (result == "succeeded")
            {
                order.Status = OrderStatus.Refunded;
                unitOfWork.Repository<Order>().Update(order);
                var success = await unitOfWork.Complete();

                if (success) return order.ToDto();
                return BadRequest("Problem updating the order status to refunded");
            }
            return BadRequest("Failed to refund the payment");
        }
    }
}
