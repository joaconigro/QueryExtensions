namespace JSoft.QueryExtensions
{
    public abstract class NumericFilterCondition : FilterCondition
    {
        protected NumericFilterCondition() { }
        protected NumericFilterCondition(string property, string operation)
        {
            Property = property;
            Operation = operation switch
            {
                "equals" => NumericOperations.Equals,
                "notEqual" => NumericOperations.NotEqual,
                "lessThan" => NumericOperations.LessThan,
                "lessThanOrEqual" => NumericOperations.LessThanOrEqual,
                "greaterThan" => NumericOperations.GreaterThan,
                "greaterThanOrEqual" => NumericOperations.GreaterThanOrEqual,
                "inRange" => NumericOperations.InRange,
                _ => NumericOperations.Equals
            };
        }

        public NumericOperations Operation { get; set; }
    }
}
