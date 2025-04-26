using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;

namespace Core.Specifications
{
    public class BaseSpecification<T>(Expression<Func<T, bool>>? Criteria) : ISpecification<T>
    {
        public BaseSpecification() : this(null) { }
        public Expression<Func<T, bool>>? WhereExpression => Criteria;

        public Expression<Func<T, object>>? OrderByExpression { get; private set; }

        public Expression<Func<T, object>>? OrderByDescExpression { get; private set; }

        public bool IsDistinct { get; private set; }

        protected void AddOrderBy(Expression<Func<T, object>>? OrderBy)
        {
            OrderByExpression = OrderBy;
        }
        protected void AddOrderByDesc(Expression<Func<T, object>>? OrderByDesc)
        {
            OrderByDescExpression = OrderByDesc;
        }
        protected void ApplyDistinct()
        {
            IsDistinct = true;
        }
    }

    public class BaseSpecification<T, TResult>(Expression<Func<T, bool>>? Criteria)
        : BaseSpecification<T>(Criteria), ISpecification<T, TResult>
    {
        public BaseSpecification() : this(null)
        {
            
        }
        public Expression<Func<T, TResult>>? SelectExpression {  get; private set; }

        protected void AddSelect(Expression<Func<T, TResult>> select)
        {
            SelectExpression = select;
        }
    }
}
