using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification(ProductSpecParameters specParameters) : base(x =>
            (!specParameters.brands.Any() || specParameters.brands.Contains(x.Brand)) &&
            (!specParameters.types.Any() || specParameters.types.Contains(x.Type)) &&
            (string.IsNullOrEmpty(specParameters.Search) || x.Name.ToLower().Contains(specParameters.Search))
        )
        {
            ApplyPaging(specParameters.pageSize * (specParameters.pageIndex - 1), specParameters.pageSize);

            switch (specParameters.Sort)
            {
                case "priceAsc":
                    AddOrderBy(x => x.Price);
                    break;
                case "priceDesc":
                    AddOrderByDesc(x => x.Price);
                    break;
                default:
                    AddOrderBy(x => x.Name);
                    break;
            }

        }

    }
}
