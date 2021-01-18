using System;
using System.Linq.Expressions;
using System.Reflection;

namespace JSoft.QueryExtensions
{
    /// <summary>
    /// Defines the <see cref="FilterCondition" />.
    /// </summary>
    public abstract class FilterCondition : IFilterCondition
    {
        /// <summary>
        /// Gets or sets the property name used to filter. It's case insensitive.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Returns a clone of this condition.
        /// </summary>
        /// <returns>The <see cref="IFilterCondition"/>.</returns>
        public abstract IFilterCondition Clone();

        /// <summary>
        /// Returns a Lambda Expression that can be used to filter a IQueryable, even on server side. Returns null if the property wasn't found.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the property.</param>
        /// <param name="parameter">Represents a named parameter expression. See <see cref="ParameterExpression"/>.</param>
        /// <returns>The <see cref="Expression{Func{T, bool}}"/>.</returns>
        public abstract Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter);

        /// <summary>
        /// Gets or sets the MemberExpression.
        /// </summary>
        protected MemberExpression MemberExpression { get; set; }

        /// <summary>
        /// Gets or sets the PropertyInfo.
        /// </summary>
        protected PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Gets the <see cref="System.Reflection.PropertyInfo"/> and <see cref="System.Linq.Expressions.MemberExpression"/> properties.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the property.</param>
        /// <param name="parameter">Represents a named parameter expression. See <see cref="ParameterExpression"/>.</param>
        /// <returns>True if the property was found. False otherwise.</returns>
        protected bool TryGetPropertyAndMember(Type type, ParameterExpression parameter)
        {
            //Try to get property info object for the property name.
            PropertyInfo = type.GetProperty(Property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (PropertyInfo == null)
            {
                return false;
            }

            MemberExpression = Expression.MakeMemberAccess(parameter, PropertyInfo);
            return true;
        }

        /// <summary>
        /// Returns true if the Property if type of <see cref="Nullable{T}"/>.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        protected bool IsNullableType()
        {
            if (PropertyInfo == null)
            {
                return false;
            }
            return PropertyInfo.PropertyType.IsGenericType && PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    /// <summary>
    /// Defines the NumericOperations.
    /// </summary>
    public enum NumericOperations
    {
        /// <summary>
        /// Defines the Equals.
        /// </summary>
        Equals,

        /// <summary>
        /// Defines the NotEqual.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Defines the LessThan.
        /// </summary>
        LessThan,

        /// <summary>
        /// Defines the LessThanOrEqual.
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Defines the GreaterThan.
        /// </summary>
        GreaterThan,

        /// <summary>
        /// Defines the GreaterThanOrEqual.
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Defines the InRange.
        /// </summary>
        InRange
    }

    /// <summary>
    /// Defines the StringOperations.
    /// </summary>
    public enum StringOperations
    {
        /// <summary>
        /// Defines the Contains.
        /// </summary>
        Contains,

        /// <summary>
        /// Defines the NotContains.
        /// </summary>
        NotContains,

        /// <summary>
        /// Defines the Equals.
        /// </summary>
        Equals,

        /// <summary>
        /// Defines the NotEqual.
        /// </summary>
        NotEqual,

        /// <summary>
        /// Defines the StartsWith.
        /// </summary>
        StartsWith,

        /// <summary>
        /// Defines the EndsWith.
        /// </summary>
        EndsWith
    }
}
