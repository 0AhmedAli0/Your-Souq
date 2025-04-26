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

            if (spec.WhereExpression != null)
                query = query.Where(spec.WhereExpression);

            if (spec.OrderByExpression != null)
                query = query.OrderBy(spec.OrderByExpression);

            if (spec.OrderByDescExpression != null)
                query = query.OrderByDescending(spec.OrderByDescExpression);

            if (spec.IsDistinct)
                query = query.Distinct();

            return query;
        }

        public static IQueryable<TResult> GetQuery<TSpec,TResult>(IQueryable<T> inputQuery ,ISpecification<T, TResult> spec )
        {
            var query = inputQuery;

            if (spec.WhereExpression != null)
                query = query.Where(spec.WhereExpression);

            if (spec.OrderByExpression != null)
                query = query.OrderBy(spec.OrderByExpression);

            if (spec.OrderByDescExpression != null)
                query = query.OrderByDescending(spec.OrderByDescExpression);

            var SelectQuery = query as IQueryable<TResult>;

            if (spec.SelectExpression != null)
             SelectQuery = query.Select(spec.SelectExpression);

            if (spec.IsDistinct)
                SelectQuery = SelectQuery?.Distinct();

            return SelectQuery ?? query.Cast<TResult>();
        }
    }
}
