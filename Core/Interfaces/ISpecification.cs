﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface ISpecification<T> 
    {
        public Expression<Func<T,bool>>? WhereExpression { get; }//Criteria
        public Expression<Func<T,object>>? OrderByExpression { get; }
        public Expression<Func<T,object>>? OrderByDescExpression { get; }
        bool IsDistinct { get; }
        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
        IQueryable<T> ApplyCriteria(IQueryable<T> query);

    }
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        public Expression<Func<T, TResult>>? SelectExpression { get; }

    }
}
