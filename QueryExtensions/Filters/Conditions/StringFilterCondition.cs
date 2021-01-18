using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class StringFilterCondition : FilterCondition
    {
        /// <summary>
        /// Creates a new instance of <see cref="StringFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive</param>
        /// <param name="operation">The operation to compare.</param>
        public StringFilterCondition(string property, string filter, StringOperations operation)
        {
            Property = property;
            Filter = filter.ToLower();
            Operation = operation;
        }

        /// <summary>
        /// Creates a new instance of <see cref="StringFilterCondition"/>. Valid operation values are: equals, notEqual, startsWith, endsWith, notContains and contains.
        /// </summary>
        /// <param name="property">The property name, case insensitive</param>
        /// <param name="operation">Valid operation values are: equals, notEqual, startsWith, endsWith, notContains and contains.<br/>
        /// If operation value isn't any of these values, "contains" value will be used.</param>
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

        private string Filter { get; set; }
        private StringOperations Operation { get; set; }

        public override Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            //Try to get property info an member access expression. If returns false, the expression will be null.
            if (!TryGetPropertyAndMember(type, parameter))
            {
                return null;
            }

            //Gets the strimg methods.
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

            //Creates the expressions to compare with filter values, always using ToLower().
            var constant = Expression.Constant(Filter);
            var toLower = Expression.Call(MemberExpression, toLowerMethod);

            //Creates the body expression.
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
            return new StringFilterCondition(Property, Filter, Operation);
        }
    }
}
