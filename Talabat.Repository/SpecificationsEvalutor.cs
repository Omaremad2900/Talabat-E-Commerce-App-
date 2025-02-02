﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public class SpecificationsEvalutor<T> where T: BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> Spec)
        {
            var Query = inputQuery;
            if (Spec.Criteria != null)
            { 
             Query=Query.Where(Spec.Criteria);
            }
            if(Spec.OrderBy != null) {  Query=Query.OrderBy(Spec.OrderBy);}
            if(Spec.OrderByDescending != null) { Query=Query.OrderByDescending(Spec.OrderByDescending);}
            if(Spec.IsPaginationEnabled)
                { Query=Query.Skip(Spec.Skip).Take(Spec.Take); }
            Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpressions) => CurrentQuery.Include(IncludeExpressions));
            return Query;
         
        }
    }
}
