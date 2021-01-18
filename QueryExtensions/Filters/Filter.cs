using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    /// <summary>
    /// Defines the <see cref="Filter" />.
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Returns true if there is more than one condition.
        /// </summary>
        public bool HasMultipleConditions => Conditions?.Count > 1;

        /// <summary>
        /// Gets or sets the Operator.
        /// </summary>
        public Operators Operator { get; set; }

        /// <summary>
        /// A list of all conditions used to filter.
        /// </summary>
        public List<IFilterCondition> Conditions { get; private set; }

        /// <summary>
        /// Gets or sets the property name used to filter.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter"/> class.
        /// </summary>
        public Filter()
        {
            Conditions = new List<IFilterCondition>();
        }

        /// <summary>
        /// Returns a Lambda Expression merging all the conditions, that can be used to filter a IQueryable, even on server side.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the property.</param>
        /// <param name="parameter">Represents a named parameter expression. See <see cref="ParameterExpression"/>.</param>
        /// <returns>The <see cref="Expression{Func{T, bool}}"/>.</returns>
        public Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            if (HasMultipleConditions)
            {
                Expression<Func<T, bool>> expression = null;
                foreach (var condition in Conditions)
                {
                    var expr = condition.GetLambdaExpression<T>(type, parameter);
                    expression = expression == null ? expr : FilterExtensions.CombineExpressions<T>(expression, expr, Operator, parameter);
                }
                return expression;
            }
            else
            {
                return Conditions[0].GetLambdaExpression<T>(type, parameter);
            }
        }

        /// <summary>
        /// Retunrs a copy of this filter.
        /// </summary>
        /// <returns>The <see cref="Filter"/>.</returns>
        public Filter Copy()
        {
            var f = new Filter()
            {
                Operator = Operator
            };

            f.Conditions = Conditions.Select(c => c.Clone()).ToList();
            return f;
        }

        /// <summary>
        /// Adds a new condition to the list of conditions.
        /// </summary>
        /// <param name="condition">The <see cref="IFilterCondition"/> condition.</param>
        public void Add(IFilterCondition condition)
        {
            Conditions.Add(condition);
        }

        /// <summary>
        /// Adds a range of condition to the list of conditions.
        /// </summary>
        /// <param name="conditions">The <see cref="IEnumerable{IFilterCondition}"/> conditions.</param>
        public void AddRange(IEnumerable<IFilterCondition> conditions)
        {
            Conditions.AddRange(conditions);
        }
    }

    /// <summary>
    /// Defines the Operators.
    /// </summary>
    public enum Operators
    {
        /// <summary>
        /// The And operator (&& in C# or AndAlso in VB.NET).
        /// </summary>
        And,

        /// <summary>
        /// The Or operator (|| in C# or OrElse in VB.NET).
        /// </summary>
        Or
    }
}
