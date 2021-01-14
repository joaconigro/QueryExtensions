using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace JSoft.QueryExtensions
{
    public class Filter
    {
        public bool HasMultipleConditions => Conditions?.Count > 1;
        public Operators Operator { get; set; }
        public List<IFilterCondition> Conditions { get; set; }
        public string Property { get; set; }


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


        public static List<Filter> ParseAgGridFilters(string filters)
        {
            var dataFilters = new List<Filter>();
            if (!string.IsNullOrEmpty(filters))
            {
                var dict = JObject.Parse(filters);
                foreach (var token in dict.Children())
                {
                    dataFilters.Add(ParseTokenToFilter(token));
                }
            }
            return dataFilters;
        }

        static Filter ParseTokenToFilter(JToken token)
        {
            var filter = new Filter
            {
                Property = token.Path
            };
            filter.Conditions = ParseTokenToCondition(token.Path, filter, token.Children().AsEnumerable());
            return filter;
        }

        static List<IFilterCondition> ParseTokenToCondition(string propName, Filter filter, IEnumerable<JToken> tokens)
        {
            var conditions = new List<IFilterCondition>();
            foreach (var token in tokens)
            {
                var d = token.ToObject<Dictionary<string, object>>();
                if (d.ContainsKey("filterType") && !d.ContainsKey("operator"))
                {
                    conditions = CreateFilterCondition(propName, d, filter);
                }
                else
                {
                    conditions.AddRange(ParseTokenToCondition(propName, filter, (new JToken[] { JToken.FromObject(d["condition1"]) })));
                    conditions.AddRange(ParseTokenToCondition(propName, filter, (new JToken[] { JToken.FromObject(d["condition2"]) })));
                    if (filter != null)
                    {
                        filter.Operator = d["operator"].ToString().ToUpper() == "AND" ? Operators.And : Operators.Or;
                    }
                }
            }
            return conditions;
        }

        static List<IFilterCondition> CreateFilterCondition(string propName, Dictionary<string, object> dictionary, Filter filter)
        {
            var conditions = new List<IFilterCondition>();
            switch (dictionary["filterType"])
            {
                case "text":
                    conditions.Add(new StringFilterCondition(propName, dictionary["filter"].ToString(), dictionary["type"].ToString()));
                    break;
                case "number":
                    conditions.Add(new DoubleFilterCondition(propName, dictionary["filter"].ToString(),
                        dictionary.ContainsKey("filterTo") ? dictionary["filterTo"]?.ToString() : null, dictionary["type"].ToString()));
                    break;
                case "date":
                    conditions.Add(new DateTimeFilterCondition(propName, dictionary["dateFrom"].ToString(),
                       dictionary.ContainsKey("dateTo") ? dictionary["dateTo"]?.ToString() : null, dictionary["type"].ToString()));
                    break;
                case "bool":
                    conditions.Add(new BooleanFilterCondition(propName, dictionary["filter"].ToString()));
                    break;
                case "enum":
                    var values = dictionary["filter"].ToString().Split(";");
                    conditions.AddRange(values.Select(s => new EnumFilterCondition(propName, s)));
                    if (filter != null)
                    {
                        filter.Operator = Operators.Or;
                    }
                    break;
            }

            return conditions;
        }
    }

    public enum Operators
    {
        And,
        Or
    }
}
