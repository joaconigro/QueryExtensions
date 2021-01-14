using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JSoft.QueryExtensions
{
    public static class SortExtensions
    {
        private class OrderOptions
        {
            public string PropertyName { get; set; }
            public string OrderBy { get; set; }
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string orderString)
        {
            if (string.IsNullOrEmpty(orderString) || orderString == "null")
            {
                return source;
            }

            var type = typeof(T);
            var options = ParseOrdering(orderString);
            var property = type.GetProperty(options.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return source;
            }

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), options.OrderBy, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> source, string orderString)
        {
            if (string.IsNullOrEmpty(orderString) || orderString == "null")
            {
                return source;
            }

            return source.AsQueryable().Sort(orderString).AsEnumerable();
        }

        private static OrderOptions ParseOrdering(string orderString)
        {
            var values = orderString.Split(";");
            string prop;
            string orderby;

            if (values.Length > 1)
            {
                prop = values[0];
                orderby = values[1].ToLower().Contains("asc") ? "OrderBy" : "OrderByDescending";
            }
            else
            {
                prop = orderString;
                orderby = "OrderBy";
            }

            return new OrderOptions()
            {
                PropertyName = prop,
                OrderBy = orderby
            };
        }
    }
}
