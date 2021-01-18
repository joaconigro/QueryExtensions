using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    /// <summary>
    /// Defines the <see cref="FilterExtensions" />.
    /// </summary>
    public static class FilterExtensions
    {
        /// <summary>
        /// Filters a <see cref="IQueryable{T}"/> using the filters passed as parameter.
        /// </summary>
        /// <param name="source">The <see cref="IQueryable{T}"/> source.</param>
        /// <param name="filters">The filters that will be used to filter the source.</param>
        /// <returns>The filtered <see cref="IQueryable{T}"/>. If filters is null or empty, the same source will be returned.</returns>
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

        /// <summary>
        /// Filters a <see cref="IEnumerable{T}"/> using the filters passed as parameter.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> source.</param>
        /// <param name="filters">The filters that will be used to filter the source.</param>
        /// <returns>The filtered <see cref="IEnumerable{T}"/>. If filters is null or empty, the same source will be returned.</returns>
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

        /// <summary>
        /// Combine two expressions, using the operator passed and the parameter access expression.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="combinator">The <see cref="Operators"/> combination.</param>
        /// <param name="parameter">The parameter access expression.</param>
        /// <returns>The <see cref="Expression{Func{T, bool}}"/>.</returns>
        public static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, Operators combinator, ParameterExpression parameter)
        {
            return combinator == Operators.And ?
                Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, expr2.Body), parameter) :
                Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, expr2.Body), parameter);
        }

        /// <summary>
        /// Gets the final expression, combining all the filters.
        /// </summary>
        /// <param name="filters">The <see cref="IEnumerable{Filter}"/> filters.</param>
        /// <returns>The <see cref="Expression{Func{T, bool}}"/>.</returns>
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
