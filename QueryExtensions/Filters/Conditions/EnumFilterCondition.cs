using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class EnumFilterCondition : FilterCondition
    {
        /// <summary>
        /// Creates a new instance of <see cref="EnumFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        public EnumFilterCondition(string property, string filter)
        {
            Property = property;
            Filter = int.Parse(filter);
        }

        /// <summary>
        /// Creates a new instance of <see cref="EnumFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="filter">The value used to filter.</param>
        public EnumFilterCondition(string property, int filter)
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

            //Parse the int to the enum type and creates the constant and the body expressions.
            var en = Enum.ToObject(PropertyInfo.PropertyType, Filter);
            var constant1 = Expression.Constant(en);
            var body = Expression.Equal(MemberExpression, constant1);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public override IFilterCondition Clone()
        {
            return new EnumFilterCondition(Property, Filter);
        }

        int Filter { get; set; }
    }
}
