using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public class Filter
    {
        public bool HasMultipleConditions => Conditions?.Count > 1;
        public Operators Operator { get; set; }
        public List<IFilterCondition> Conditions { get; private set; }
        public string Property { get; set; }

        public Filter()
        {
            Conditions = new List<IFilterCondition>();
        }

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

        public Filter Copy()
        {
            var f = new Filter()
            {
                Operator = Operator
            };

            f.Conditions = Conditions.Select(c => c.Clone()).ToList();
            return f;
        }

        public void Add(IFilterCondition condition)
        {
            Conditions.Add(condition);
        }

        public void AddRange(IEnumerable<IFilterCondition> conditions)
        {
            Conditions.AddRange(conditions);
        }

    }

    public enum Operators
    {
        And,
        Or
    }
}
