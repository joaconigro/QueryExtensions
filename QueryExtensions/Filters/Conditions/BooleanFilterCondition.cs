using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class BooleanFilterCondition : FilterCondition
    {
        /// <summary>
        /// Creates a new instance of <see cref="BooleanFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        public BooleanFilterCondition(string property, string filter)
        {
            Property = property;
            Filter = bool.Parse(filter);
        }

        /// <summary>
        /// Creates a new instance of <see cref="BooleanFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        public BooleanFilterCondition(string property, bool filter)
        {
            Property = property;
            Filter = filter;
        }

        public override Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            //Try to get property info an member access expression. If returns false, the expression will be null.
            if (!TryGetPropertyAndMember(type, parameter))
            {
                return null;
            }

            //Creates the constant and the body expressions.
            var constant1 = Expression.Constant(Filter);
            var body = Expression.Equal(MemberExpression, constant1);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public override IFilterCondition Clone()
        {
            return new BooleanFilterCondition(Property, Filter);
        }

        private bool Filter { get; set; }
    }
}
