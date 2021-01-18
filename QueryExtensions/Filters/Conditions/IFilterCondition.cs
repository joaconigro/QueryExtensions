using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    /// <summary>
    /// Defines the <see cref="IFilterCondition"/> interface.
    /// </summary>
    public interface IFilterCondition
    {
        /// <summary>
        /// Gets or sets the property name used to filter. It's case insensitive.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Returns a clone of this condition.
        /// </summary>
        /// <returns>The <see cref="IFilterCondition"/>.</returns>
        IFilterCondition Clone();

        /// <summary>
        /// Returns a Lambda Expression that can be used to filter a IQueryable, even on server side. Returns null if the property wasn't found.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the property.</param>
        /// <param name="parameter">Represents a named parameter expression. See <see cref="ParameterExpression"/>.</param>
        /// <returns>The <see cref="Expression{Func{T, bool}}"/>.</returns>
        Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter);
    }
}
