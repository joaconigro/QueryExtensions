using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class BooleanFilterCondition : FilterCondition
    {
        public BooleanFilterCondition() { }
        public BooleanFilterCondition(string property, string filter)
        {
            Property = property;
            Filter = bool.Parse(filter);
        }

        public override Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            if (!GetPropertyAndMember(type, parameter))
            {
                return null;
            }
            var constant1 = Expression.Constant(Filter);
            var body = Expression.Equal(MemberExpression, constant1);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public override IFilterCondition Clone()
        {
            return new BooleanFilterCondition() { Property = Property, Filter = Filter };
        }

        public bool Filter { get; set; }
    }
}
