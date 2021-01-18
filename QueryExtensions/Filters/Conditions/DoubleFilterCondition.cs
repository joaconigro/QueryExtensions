using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class DoubleFilterCondition : NumericFilterCondition
    {
        /// <summary>
        /// Creates a new instance of <see cref="DoubleFilterCondition"/>. Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        /// <param name="operation">Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.</param>
        public DoubleFilterCondition(string property, double? filter, string operation) : base(property, operation)
        {
            Filter = filter;
            FilterTo = null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DoubleFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        /// <param name="operation">The operation to compare.</param>
        public DoubleFilterCondition(string property, double? filter, NumericOperations operation) : base(property, operation)
        {
            Filter = filter;
            FilterTo = null;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DoubleFilterCondition"/>. Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        /// <param name="filterTo">The other value used to filter. Used when the operation == "inRange"</param>
        /// <param name="operation">Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.</param>
        public DoubleFilterCondition(string property, double? filter, double? filterTo, string operation) : base(property, operation)
        {
            Filter = filter;
            FilterTo = filterTo;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DoubleFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        /// <param name="filterTo">The other value used to filter. Used when the operation == "inRange"</param>
        /// <param name="operation">The operation to compare.</param>
        public DoubleFilterCondition(string property, double? filter, double? filterTo, NumericOperations operation) : base(property, operation)
        {
            Filter = filter;
            FilterTo = filterTo;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DoubleFilterCondition"/>. Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        /// <param name="filterTo">The other value used to filter. Used when the operation == "inRange"</param>
        /// <param name="operation">Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.</param>
        public DoubleFilterCondition(string property, string filter, string filterTo, string operation) : base(property, operation)
        {
            ParseFiltersValues(filter, filterTo);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DoubleFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        /// <param name="filterTo">The other value used to filter. Used when the operation == "inRange"</param>
        /// <param name="operation">The operation to compare.</param>
        public DoubleFilterCondition(string property, string filter, string filterTo, NumericOperations operation) : base(property, operation)
        {
            ParseFiltersValues(filter, filterTo);
        }

        void ParseFiltersValues(string filter, string filterTo)
        {
            var result = double.TryParse(filter, out double value);
            if (result)
            {
                Filter = value;
            }
            else
            {
                Filter = null;
            }

            result = double.TryParse(filterTo, out value);
            if (result)
            {
                FilterTo = value;
            }
            else
            {
                FilterTo = null;
            }
        }

        public override Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            //Try to get property info an member access expression. If returns false, the expression will be null.
            if (!TryGetPropertyAndMember(type, parameter))
            {
                return null;
            }

            //Creates two constants expressions to compare with filter values.
            Expression constant1, constant2;
            if (IsNullableType())
            {
                constant1 = Expression.Convert(Expression.Constant(Filter), MemberExpression.Type);
                constant2 = Expression.Convert(Expression.Constant(FilterTo), MemberExpression.Type);
            }
            else
            {
                constant1 = Expression.Constant(Filter);
                constant2 = Expression.Constant(FilterTo);
            }

            //Creates the body expression.
            var body = Operation switch
            {
                NumericOperations.GreaterThan => Expression.GreaterThan(MemberExpression, constant1),
                NumericOperations.GreaterThanOrEqual => Expression.GreaterThanOrEqual(MemberExpression, constant1),
                NumericOperations.InRange => Expression.AndAlso(Expression.GreaterThanOrEqual(MemberExpression, constant1), Expression.LessThanOrEqual(MemberExpression, constant2)),
                NumericOperations.LessThan => Expression.LessThan(MemberExpression, constant1),
                NumericOperations.LessThanOrEqual => Expression.LessThanOrEqual(MemberExpression, constant1),
                NumericOperations.NotEqual => Expression.NotEqual(MemberExpression, constant1),
                _ => Expression.Equal(MemberExpression, constant1)
            };

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public override IFilterCondition Clone()
        {
            return new DoubleFilterCondition(Property, Filter, FilterTo, Operation);
        }

        /// <summary>
        /// Gets or sets the Filter value.
        /// </summary>
        private double? Filter { get; set; }

        /// <summary>
        /// Gets or sets the FilterTo value.
        /// </summary>
        private double? FilterTo { get; set; }
    }
}
