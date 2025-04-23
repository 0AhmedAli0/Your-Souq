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
    public class BaseSpecification<T>(Expression<Func<T, bool>> Criteria) : ISpecification<T>
    {
        public Expression<Func<T, bool>> WhereExpression => Criteria;
    }
}
