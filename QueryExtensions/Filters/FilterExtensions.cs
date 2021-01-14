using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public static class FilterExtensions
    {

        public static IQueryable<T> Filter<T>(this IQueryable<T> source, IEnumerable<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return source;
            }

            Expression<Func<T, bool>> expression = GetExpression<T>(filters);
            if (expression == null)
            {
                return source;
            }

            return source.Where(expression);
        }

        public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, IEnumerable<Filter> filters)
        {
            if (filters == null || !filters.Any())
            {
                return source;
            }

            Expression<Func<T, bool>> expression = GetExpression<T>(filters);
            if (expression == null)
            {
                return source;
            }

            return source.AsQueryable().Where(expression).AsEnumerable();
        }

        public static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, Operators combinator, ParameterExpression parameter)
        {
            return combinator == Operators.And ? 
                Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, expr2.Body), parameter) : 
                Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, expr2.Body), parameter);
        }

        static Expression<Func<T, bool>> GetExpression<T>(IEnumerable<Filter> filters)
        {
            Expression<Func<T, bool>> expression = null;
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            foreach (var filter in filters)
            {
                var expr = filter.GetLambdaExpression<T>(type, parameter);
                expression = expression == null ? expr : CombineExpressions<T>(expression, expr, Operators.And, parameter);
            }

            return expression;
        }
    }
}
