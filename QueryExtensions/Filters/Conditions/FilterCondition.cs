using System;
using System.Linq.Expressions;
using System.Reflection;

namespace JSoft.QueryExtensions
{
    public abstract class FilterCondition : IFilterCondition
    {
        public string Property { get; set; }
        public abstract IFilterCondition Clone();
        public abstract Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter);

        protected MemberExpression MemberExpression { get; set; }
        protected PropertyInfo PropertyInfo { get; set; }

        protected bool GetPropertyAndMember(Type type, ParameterExpression parameter)
        {
            PropertyInfo = type.GetProperty(Property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (PropertyInfo == null)
            {
                return false;
            }

            MemberExpression = Expression.MakeMemberAccess(parameter, PropertyInfo);
            return true;
        }

        protected bool IsNullableType()
        {
            if (PropertyInfo == null)
            {
                return false;
            }
            return PropertyInfo.PropertyType.IsGenericType && PropertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }

    public enum NumericOperations
    {
        Equals,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        InRange
    }

    public enum StringOperations
    {
        Contains,
        NotContains,
        Equals,
        NotEqual,
        StartsWith,
        EndsWith
    }
}
