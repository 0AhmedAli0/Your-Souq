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
    public class PaymentService(IConfiguration config,ICartService cartService,
        IUnitOfWork unitOfWork) : IPaymentService
    {
    

        public async Task<ShoppingCart?> CreateOrUpdatePaymentIntent(string cardId)
        {
            StripeConfiguration.ApiKey = config["StripeSettings:SecretKey"];

            //1-get basket
            var cart = await cartService.GetCartAsync(cardId);
            if (cart==null) return null;


            //2-get delivery cost
            var shippingPrice = 0m;
            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync((int)cart.DeliveryMethodId);
                if (deliveryMethod == null) return null;
                shippingPrice = deliveryMethod.Price;
            }

            //3-هنا بظبط السعر لو هو جايلي من الفرونت مش مظبوط
            foreach (var item in cart.Items)
            {
                var product = await unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.ProductId);
                if (product == null) return null; // If product not found, return null
                if (item.Price != product.Price)
                {
                    item.Price = product.Price;
                }
            }

            //4- create or update payment intent
            var service = new PaymentIntentService();
            PaymentIntent? intent = null;
                //check if payment intent already exists
            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)((cart.Items.Sum(i => i.Price * i.Quantity) + shippingPrice) * 100), // Stripe expects amount in cents
                    Currency = "usd",
                    PaymentMethodTypes = ["card" ]

                };
                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;// this is used by the client to communicate  with stripe to cofirm payment
            }   //update payment intent
            else 
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)((cart.Items.Sum(i => i.Price * i.Quantity) + shippingPrice) * 100), // Stripe expects amount in cents
                };
                intent = await service.UpdateAsync(cart.PaymentIntentId, options);
            }

            //5- Update cart
            await cartService.SetCartAsync(cart);
            return cart;

        }
    }
}
