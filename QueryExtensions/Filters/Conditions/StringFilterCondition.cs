using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class StringFilterCondition : FilterCondition
    {
        public StringFilterCondition() { }

        public StringFilterCondition(string property, string filter, string operation)
        {
            Property = property;
            Filter = filter.ToLower();
            Operation = operation switch
            {
                "notContains" => StringOperations.NotContains,
                "equals" => StringOperations.Equals,
                "notEqual" => StringOperations.NotEqual,
                "startsWith" => StringOperations.StartsWith,
                "endsWith" => StringOperations.EndsWith,
                _ => StringOperations.Contains
            };
        }
        public string Filter { get; set; }
        public StringOperations Operation { get; set; }

        public override Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            if (!GetPropertyAndMember(type, parameter))
            {
                return null;
            }

            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

            var constant = Expression.Constant(Filter);
            var toLower = Expression.Call(MemberExpression, toLowerMethod);

            Expression body = Operation switch
            {
                StringOperations.NotContains => Expression.Not(Expression.Call(toLower, containsMethod, constant)),
                StringOperations.Equals => Expression.Equal(toLower, constant),
                StringOperations.NotEqual => Expression.NotEqual(toLower, constant),
                StringOperations.StartsWith => Expression.Call(toLower, startsWithMethod, constant),
                StringOperations.EndsWith => Expression.Call(toLower, endsWithMethod, constant),
                _ => Expression.Call(toLower, containsMethod, constant)
            };

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public override IFilterCondition Clone()
        {
            return new StringFilterCondition() { Property = Property, Filter = Filter, Operation = Operation };
        }
    }
}
