using API.Extensions;
using API.SignalR;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Stripe;

namespace API.Controllers
{

    public class PaymentsController(IPaymentService paymentService,
        IUnitOfWork unitOfWork, ILogger<PaymentsController> logger, IConfiguration configuration
        ,IHubContext<NotificationHub> hubContext) : BaseApiController
    {
        private readonly string _whSecret = configuration["StripeSettings:WhSecret"]!;

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

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {////stripe listen --forward-to https://localhost:5001/api/payments/webhook -e payment_intent.succeeded

            var json = await new StreamReader(Request.Body).ReadToEndAsync();//to get the events that stripe sends to us(from request body)
            try
            {//get data from this event to use it in our code
                var stripeEvent = constructStripeEvent(json);//greate stripe event from json

                if (stripeEvent.Data.Object is not PaymentIntent intent)
                {
                    logger.LogError("Invalid Stripe event data");
                    return BadRequest("Invalid event data");
                }

                await HandlePaymentIntentSucceeded(intent);//handle the payment intent succeeded event
                return Ok();
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "Stripe error processing webhook");
                return StatusCode(StatusCodes.Status500InternalServerError, "Stripe error processing webhook");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing Stripe webhook");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error processing Stripe webhook");
            }

        }

        private async Task HandlePaymentIntentSucceeded(PaymentIntent intent)
        {
            if (intent.Status == "succeeded")
            {
                //get order by intent Id
                var spec = new OrderSpecification(intent.Id, true);

                var order = await unitOfWork.Repository<Order>().GetEntityWithSpec(spec)
                    ?? throw new Exception("Order not found");

                if ((long)order.GetTotal() * 100 != intent.AmountReceived)
                {
                    order.Status = OrderStatus.PaymentMismatch;
                    logger.LogError("Payment amount does not match order total");
                    throw new Exception("Payment amount does not match order total");
                }
                else
                {
                    order.Status = OrderStatus.PaymentReceived;
                }

                await unitOfWork.Complete();
                //SignalsR to update the order status in the client
                var connectionId = NotificationHub.GetConnectionIdByEmail(order.BuyerEmail);
                if (!string.IsNullOrEmpty(connectionId))
                {
                    await hubContext.Clients.Client(connectionId)
                        .SendAsync("OrderCompleteNotification", order.ToDto());// هذا هو اسم الأشعار الذي سيتم الاستماع اليه من جانب العميل
                }
            }
        }

        private Event constructStripeEvent(string json)
        {
            try
            {
                // Use the Stripe library to construct the event
                return EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _whSecret,
                    throwOnApiVersionMismatch: false // تعطيل التحقق من الإصدار
                );// this is the secret key that we get from stripe dashboard, to verify that the event is from stripe
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error constructing Stripe event");
                throw new StripeException("Invalide Signature");
            }
        }
    }

}

