using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public class ProductItemOrdered
    {//جدول بحتفظ بيه ببياانات المنتج اللي اتطلب بحيث لو حصل اي تغيير علي بيانات المنتج دا بعدين
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string PictureUrl { get; set; }
    }
}
