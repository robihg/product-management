using framework.BaseService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace framework.BaseService.BusinessServices
{
    public class GridPaginationService
    {
        public async Task<PagedResult<T>> GetPagedData<T>(IQueryable<T> query, PaginationParams paginationParams)
        {
            query = ApplySorting(query, paginationParams.SortColumn, paginationParams.SortDescending);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize
            };
        }

        private static IQueryable<T> ApplySorting<T>(IQueryable<T> query, string? sortColumn, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortColumn))
            {
                return query;
            }

            var param = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(param, sortColumn);
            var sortExpression = Expression.Lambda(property, param);

            var methodName = sortDescending ? "OrderByDescending" : "OrderBy";
            var callExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.Type },
                query.Expression,
                Expression.Quote(sortExpression));

            return query.Provider.CreateQuery<T>(callExpression);
        }
    }
}
