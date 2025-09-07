using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration config;
        private readonly ICartService cartService;
        private readonly IUnitOfWork unitOfWork;

        public PaymentService(IConfiguration config, ICartService cartService, IUnitOfWork unitOfWork)
        {
            this.cartService = cartService;
            this.unitOfWork = unitOfWork;
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];
        }

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cardId)
        {

            //1-get basket
            var cart = await cartService.GetCartAsync(cardId) ?? throw new Exception("Cart unavailable");


            //2-get delivery cost
            var shippingPrice = await GetShippingPriceAsync(cart) ?? 0; // If no delivery method, shipping price is 0

            //3-هنا بظبط السعر لو هو جايلي من الفرونت مش مظبوط
            await ValidateCartItemsInCartAsync(cart);

            // Calculate subtotal
            var subtotal = CalculateSubtotal(cart);

            if (cart.Coupon != null)
            {
                subtotal = await ApplyDiscountAsync(cart.Coupon, subtotal);
            }

            var total = subtotal + shippingPrice;

            //4- create or update payment intent
            await CreateUpdatePaymentIntentAsync(cart, total);

            

            //5- Update cart
            await cartService.SetCartAsync(cart);
            return cart;

        }

        public async Task<string> RefundPayment(string paymentIntentId)
        {
            var refundOptions = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            var refundService = new RefundService();

            var result = await refundService.CreateAsync(refundOptions);

            return result.Status;
        }

        private async Task CreateUpdatePaymentIntentAsync(ShoppingCart cart, long total)
        {
            var service = new PaymentIntentService();

            //check if payment intent already exists
            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = total, // Stripe expects amount in cents
                    Currency = "usd",
                    PaymentMethodTypes = ["card"]

                };
                var intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;// this is used by the client to communicate  with stripe to cofirm payment
            }  
            //update payment intent
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = total // Stripe expects amount in cents
                };
                await service.UpdateAsync(cart.PaymentIntentId, options);
            }
        }
        private async Task<long> ApplyDiscountAsync(AppCoupon appCoupon, long amount)
        {
            var couponService = new Stripe.CouponService();

            var coupon = await couponService.GetAsync(appCoupon.CouponId);

            if (coupon.AmountOff.HasValue)
            {
                amount -= (long)coupon.AmountOff * 100;
            }

            if (coupon.PercentOff.HasValue)
            {
                var discount = amount * (coupon.PercentOff.Value / 100);
                amount -= (long)discount;
            }

            return amount;
        }

        private long CalculateSubtotal(ShoppingCart cart)
        {
            var itemTotal = cart.Items.Sum(x => x.Quantity * x.Price * 100);
            return (long)itemTotal;
        }

        private async Task ValidateCartItemsInCartAsync(ShoppingCart cart)
        {
            foreach (var item in cart.Items)
            {
                var product = await unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId)
                    ?? throw new Exception("Problem getting product in cart");

                if (item.Price != product.Price)
                {
                    item.Price = product.Price;
                }
            }
        }

        private async Task<long?> GetShippingPriceAsync(ShoppingCart cart)
        {
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>()
                    .GetByIdAsync((int)cart.DeliveryMethodId)
                        ?? throw new Exception("Problem with delivery method");

                return (long)deliveryMethod.Price * 100;
            }

            return null;
        }


    }
}
