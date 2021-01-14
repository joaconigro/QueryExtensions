using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace JSoft.QueryExtensions
{
    public static class AgGridFilterParser
    {

        public static List<Filter> Parse(string filters)
        {
            var dataFilters = new List<Filter>();
            if (!string.IsNullOrEmpty(filters))
            {
                var dict = JsonDocument.Parse(filters);
                var properties = dict.RootElement.EnumerateObject().ToList();
                foreach (var property in properties)
                {
                    dataFilters.Add(ParseJsonProperty(property));
                }
            }
            return dataFilters;
        }

        static Filter ParseJsonProperty(JsonProperty property)
        {
            var filter = new Filter
            {
                Property = property.Name
            };

            switch (property.Value.ValueKind)
            {
                case JsonValueKind.Object:
                    filter.AddRange(ParseTokenToCondition(property.Name, filter, property.Value.EnumerateObject().ToList()));
                    break;
                default:
                    break;
            }

            return filter;
        }

        static List<IFilterCondition> ParseTokenToCondition(string propName, Filter filter, List<JsonProperty> properties)
        {
            var conditions = new List<IFilterCondition>();

            string filterType = properties.First(p => p.Name == "filterType").Value.GetString();

            if (properties.Exists(p => p.Name.Contains("condition")))
            {
                var logicOperator = properties.First(p => p.Name == "operator").Value.GetString();
                conditions.AddRange(ParseTokenToCondition(propName, filter, properties.First(p => p.Name == "condition1").Value.EnumerateObject().ToList()));
                conditions.AddRange(ParseTokenToCondition(propName, filter, properties.First(p => p.Name == "condition2").Value.EnumerateObject().ToList()));
                if (filter != null)
                {
                    filter.Operator = logicOperator.ToUpper() == "AND" ? Operators.And : Operators.Or;
                }
            }
            else
            {
                string operation = null;
                if (properties.Exists(p => p.Name == "type"))
                {
                    operation = properties.First(p => p.Name == "type").Value.GetString();
                }

                var value1Key = filterType == "date" ? "dateFrom" : "filter";
                var value2Key = filterType == "date" ? "dateTo" : "filterTo";
                var value1 = GetStringValue(properties.First(p => p.Name == value1Key).Value);
                string value2 = null;
                if (properties.Exists(p => p.Name == value2Key))
                {
                    value2 = GetStringValue(properties.First(p => p.Name == value2Key).Value);
                }

                conditions = CreateFilterCondition(propName, filterType, operation, value1, value2, filter);
            }

            return conditions;
        }

        static string GetStringValue(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Undefined or JsonValueKind.Null => null,
                JsonValueKind.Number => element.GetDouble().ToString(),
                JsonValueKind.String => element.GetString(),
                _ => element.GetRawText(),
            };
        }

        static List<IFilterCondition> CreateFilterCondition(string propName, string type, string operation, string value1, string value2, Filter filter)
        {
            var conditions = new List<IFilterCondition>();
            switch (type)
            {
                case "text":
                    conditions.Add(new StringFilterCondition(propName, value1, operation));
                    break;
                case "number":
                    conditions.Add(new DoubleFilterCondition(propName, value1, !string.IsNullOrEmpty(value2) ? value2 : null, operation));
                    break;
                case "date":
                    conditions.Add(new DateTimeFilterCondition(propName, value1, !string.IsNullOrEmpty(value2) ? value2 : null, operation));
                    break;
                case "bool":
                    conditions.Add(new BooleanFilterCondition(propName, value1));
                    break;
                case "enum":
                    var values = value1.Split(";");
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
}
