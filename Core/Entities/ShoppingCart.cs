using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ShoppingCart
    {
        public required string Id { get; set; }
        public List<CartItem> Items { get; set; } = [];
        public int? DeliveryMethodId { get; set; }
        public string? ClientSecret { get; set; }// stripe create this with payment intent , to use it when client wants to pay
        public string? PaymentIntentId { get; set; } 
    }
}
