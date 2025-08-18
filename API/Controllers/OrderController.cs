using System.Security.Claims;
using API.Dtos;
using API.Extensions;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class OrderController(ICartService cartService, IUnitOfWork unitOfWork) : BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto orderDto)
        {
            var cart = await cartService.GetCartAsync(orderDto.CartId);
            if (cart == null || !cart.Items.Any())
            {
                return BadRequest(new { message = "Cart is empty or does not exist." });
            }

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                return BadRequest(new { message = "Payment intent ID is required." });
            }

            var Items = new List<OrderItem>();
            foreach (var item in cart.Items) {
                var product = await unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Product with ID {item.ProductId} not found.");
                }

                Items.Add(new OrderItem
                {
                    ItemOrdered = new ProductItemOrdered
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = product.PictureUrl
                    },
                    Quantity = item.Quantity,
                    Price = product.Price
                });
            }

            var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(orderDto.DeliveryMethodId);
            if (deliveryMethod == null) return BadRequest("Invalid delivery method.");
            

            var order = new Order
            {
                BuyerEmail = User.GetEmail(),
                ShippingAddress = orderDto.ShippingAddress,
                DeliveryMethod = deliveryMethod,
                PaymentSummary = orderDto.PaymentSummary,
                OrderItems = Items,
                Subtotal = Items.Sum(x=>x.Price * x.Quantity),
                Discount = orderDto.Discount,
                PaymentIntentId = cart.PaymentIntentId
            };


            unitOfWork.Repository<Order>().Add(order);
            return await unitOfWork.Complete()
                ? order
                : BadRequest("Problem creating order.");
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersForUser()
        {
            var email = User.GetEmail();
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("User email is not available.");
            }

            var spec = new OrderSpecification(email);
            var orders = await unitOfWork.Repository<Order>().ListAsync(spec);
            var ordersToReturn = orders.Select(o=>o.ToDto()).ToList();

            return Ok(ordersToReturn);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var email = User.GetEmail();
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("User email is not available.");
            }
            var spec = new OrderSpecification(email,id);
            var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null)
            {
                return NotFound(new { message = "Order not found." });
            }

            var orderToReturn = order.ToDto();

            return Ok(orderToReturn);
        }
    }

}
