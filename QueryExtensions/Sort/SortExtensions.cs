using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace JSoft.QueryExtensions
{
    /// <summary>
    /// Defines the <see cref="SortExtensions" />.
    /// </summary>
    public static class SortExtensions
    {
        /// <summary>
        /// Defines the <see cref="OrderOptions" />.
        /// </summary>
        private class OrderOptions
        {
            /// <summary>
            /// Gets or sets the PropertyName.
            /// </summary>
            public string PropertyName { get; set; }

            /// <summary>
            /// Gets or sets the OrderBy. Can be "OrderBy" or "OrderByDescending".
            /// </summary>
            public string OrderBy { get; set; }
        }

        /// <summary>
        /// It sorts an <see cref="IQueryable{T}"/> source using the <see cref="OrderOptions"/> passed as argument.
        /// </summary>
        /// <typeparam name="T">.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/> source.</param>
        /// <param name="options">The <see cref="OrderOptions"/> options.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        static IQueryable<T> Sort<T>(this IQueryable<T> source, OrderOptions options)
        {
            if(source == null)
            {
                throw new ArgumentNullException(nameof(source), "can't be null");
            }

            //Try to get the property. If the property can't be found, the same source will be returned.
            var type = typeof(T);
            var property = type.GetProperty(options.PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return source;
            }

            //Create the necessary expressions to create the query.
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);
            MethodCallExpression resultExp = Expression.Call(typeof(Queryable), options.OrderBy, new Type[] { type, property.PropertyType }, source.Expression, Expression.Quote(orderByExp));
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// It sorts an <see cref="IQueryable{T}"/> source using a string to indicate the property used to sort and the order method (ascending or descending).<br/>
        /// If the property can't be found or it's null, the same source will be returned.<br/>
        /// Usage: <code>var p = source.Sort("MyPropertyName;desc");</code>
        /// </summary>
        /// <param name="source">The <see cref="IQueryable{T}"/> source.</param>
        /// <param name="orderString">The <see cref="string"/> must have the property name (case insensitive) used to sort. Optionally can have a string "asc" or "desc" for indicate the order method (ascending or descending).<br/>
        /// The property and the method must be separated by a semicolon (;). If the method is omitted, "asc" will be used.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string orderString)
        {
            if (string.IsNullOrEmpty(orderString) || orderString == "null")
            {
                return source;
            }

            var options = ParseOrdering(orderString);
            return source.Sort(options);
        }

        /// <summary>
        /// It sorts an <see cref="IQueryable{T}"/> source using a string to indicate the property used to sort and a boolean to indicate the order method (true = ascending, false = descending).<br/>
        /// If the property can't be found or it's null, the same source will be returned.<br/>
        /// Usage: <code>var p = source.Sort("MyPropertyName", false);</code>
        /// </summary>
        /// <param name="source">The <see cref="IQueryable{T}"/> source.</param>
        /// <param name="propertyName">The name of the property. It's case insensitive. If the property can't be found or it's null, the same source will be returned.</param>
        /// <param name="sortAscending">A boolean to indicate the order method. If true ascending will be used, and descending otherwise.</param>
        /// <returns>The <see cref="IQueryable{T}"/>.</returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string propertyName, bool sortAscending)
        {
            if (string.IsNullOrEmpty(propertyName) || propertyName == "null")
            {
                return source;
            }

            var options = ParseOrdering(propertyName, sortAscending);
            return source.Sort(options);
        }

        /// <summary>
        /// It sorts an <see cref="IEnumerable{T}"/> source using a string to indicate the property used to sort and the order method (ascending or descending).<br/>
        /// If the property can't be found or it's null, the same source will be returned.<br/>
        /// Usage: <code>var p = source.Sort("MyPropertyName;desc");</code>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> source.</param>
        /// <param name="orderString">The <see cref="string"/> must have the property name (case insensitive) used to sort. Optionally can have a string "asc" or "desc" for indicate the order method (ascending or descending).<br/>
        /// The property and the method must be separated by a semicolon (;). If the method is omitted, "asc" will be used.</param>
        /// <returns>The <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> source, string orderString)
        {
            if (string.IsNullOrEmpty(orderString) || orderString == "null")
            {
                return source;
            }

            return source.AsQueryable().Sort(orderString).AsEnumerable();
        }

        /// <summary>
        /// It sorts an <see cref="IEnumerable{T}"/> source using a string to indicate the property used to sort and a boolean to indicate the order method (true = ascending, false = descending).<br/>
        /// If the property can't be found or it's null, the same source will be returned.<br/>
        /// Usage: <code>var p = source.Sort("MyPropertyName", false);</code>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> source.</param>
        /// <param name="propertyName">The name of the property. It's case insensitive. If the property can't be found or it's null, the same source will be returned.</param>
        /// <param name="sortAscending">A boolean to indicate the order method. If true ascending will be used, and descending otherwise.</param>
        /// <returns>The <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<T> Sort<T>(this IEnumerable<T> source, string propertyName, bool sortAscending)
        {
            if (string.IsNullOrEmpty(propertyName) || propertyName == "null")
            {
                return source;
            }

            return source.AsQueryable().Sort(propertyName, sortAscending).AsEnumerable();
        }

        /// <summary>
        /// Creates an <see cref="OrderOptions"/> from a string that indicates the property used to sort and the order method (ascending or descending).<br/>
        /// The order method is optional and it can be a string "asc" or "desc" for indicate the order method (ascending or descending).<br/>
        /// The property and the method must be separated by a semicolon (;). If the method is omitted, "asc" will be used.
        /// For example: "MyPropertyName;desc" or "MyOtherProperty"
        /// </summary>
        /// <param name="orderString">The orderString value.</param>
        /// <returns>The <see cref="OrderOptions"/>.</returns>
        static OrderOptions ParseOrdering(string orderString)
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

        /// <summary>
        /// Creates an <see cref="OrderOptions"/> from a string to indicate the property used to sort and a boolean to indicate the order method (true = ascending, false = descending).
        /// </summary>
        /// <param name="propName">The name of the property. It's case insensitive.</param>
        /// <param name="sortAscending">A boolean to indicate the order method. If true ascending will be used, and descending otherwise.</param>
        /// <returns>The <see cref="OrderOptions"/>.</returns>
        static OrderOptions ParseOrdering(string propName, bool sortAscending)
        {
            return new OrderOptions()
            {
                PropertyName = propName,
                OrderBy = sortAscending ? "OrderBy" : "OrderByDescending"
            };
        }
    }
}
