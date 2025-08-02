using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.OrderAggregate;

namespace Core.Specifications
{
    public class OrderSpecification : BaseSpecification<Order>
    {
        public OrderSpecification(string UserEmail):base(o => o.BuyerEmail == UserEmail)
        {
            AddInclude(x => x.OrderItems);
            AddInclude(x => x.DeliveryMethod);
            AddOrderByDesc(o => o.OrderDate);
        }
        //this constractor for secefic order
        public OrderSpecification(string UserEmail, int OrderId) : base(o => o.BuyerEmail == UserEmail && o.Id == OrderId)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
            AddOrderByDesc(o => o.OrderDate);
        }

        public OrderSpecification(string PaymentIntentId, bool IsPaymentIntentId) : base(o => o.PaymentIntentId == PaymentIntentId)
        {
            AddInclude("OrderItems");
            AddInclude("DeliveryMethod");
        }

    }
}
