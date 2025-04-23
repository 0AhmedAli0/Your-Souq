using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery ,ISpecification<T> spec )
        {
            var query = inputQuery;

            if(spec.WhereExpression != null)
                query=query.Where(spec.WhereExpression);

            return query;
        }
    }
}
