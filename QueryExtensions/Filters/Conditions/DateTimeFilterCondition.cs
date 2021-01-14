using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class DateTimeFilterCondition : NumericFilterCondition
    {
        public DateTimeFilterCondition() { }

        public DateTimeFilterCondition(string property, string filter, string filterTo, string operation) : base(property, operation)
        {
            Property = property;
            var result = DateTime.TryParse(filter, out DateTime value);
            if (result)
            {
                Filter = value.Date;
            }
            else
            {
                Filter = null;
            }

            result = DateTime.TryParse(filterTo, out value);
            if (result)
            {
                FilterTo = value.Date;
            }
            else
            {
                FilterTo = null;
            }
        }

        public override Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter)
        {
            if (!GetPropertyAndMember(type, parameter))
            {
                return null;
            }

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

            var body = Operation switch
            {
                NumericOperations.GreaterThan => Expression.GreaterThanOrEqual(MemberExpression, constant1),
                NumericOperations.GreaterThanOrEqual => Expression.GreaterThanOrEqual(MemberExpression, constant1),
                NumericOperations.InRange => Expression.AndAlso(Expression.GreaterThanOrEqual(MemberExpression, constant1), Expression.LessThanOrEqual(MemberExpression, constant2)),
                NumericOperations.LessThan => Expression.LessThanOrEqual(MemberExpression, constant1),
                NumericOperations.LessThanOrEqual => Expression.LessThanOrEqual(MemberExpression, constant1),
                NumericOperations.NotEqual => Expression.NotEqual(MemberExpression, constant1),
                _ => Expression.Equal(MemberExpression, constant1)
            };

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        public override IFilterCondition Clone()
        {
            return new DateTimeFilterCondition() { Property = Property, Filter = Filter, FilterTo = FilterTo, Operation = Operation };
        }

        public DateTime? Filter { get; set; }
        public DateTime? FilterTo { get; set; }
    }
}
