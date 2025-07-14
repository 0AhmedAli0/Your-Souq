using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public class OrderItem : BaseEntity
    {
        public ProductItemOrdered ItemOrdered { get; set; } = null!;
        /*
             ! هي "null-forgiving operator" في C# 8.0 فما فوق
            هذه العلامة تخبر المترجم أننا نعرف أن القيمة قد تكون  null ونحن نتحمل المسؤولية
            بدونها، سيطالبنا المترجم بمعالجة حالة القيمة الفارغة إذا كان تفعيل "nullable reference types" مفعلًا
         */
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }
}
