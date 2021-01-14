using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class EnumFilterCondition : FilterCondition
    {
        public EnumFilterCondition() { }

        public EnumFilterCondition(string property, string filter)
        {
            Property = property;
            Filter = int.Parse(filter);
        }

        public override Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            if (!GetPropertyAndMember(type, parameter))
            {
                return null;
            }

            var en = Enum.ToObject(PropertyInfo.PropertyType, Filter);
            var constant1 = Expression.Constant(en);
            var body = Expression.Equal(MemberExpression, constant1);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public override IFilterCondition Clone()
        {
            return new EnumFilterCondition() { Property = Property, Filter = Filter };
        }

        public int Filter { get; set; }
    }
}
