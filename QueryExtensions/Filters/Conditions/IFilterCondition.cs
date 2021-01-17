using System;
using System.Linq.Expressions;

namespace JSoft.QueryExtensions
{
    public interface IFilterCondition
    {
        public string Property { get; set; }
        IFilterCondition Clone();
        Expression<Func<T, bool>> GetLambdaExpression<T>(Type type, ParameterExpression parameter);
    }
}
