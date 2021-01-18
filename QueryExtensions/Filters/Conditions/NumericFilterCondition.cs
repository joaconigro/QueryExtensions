namespace JSoft.QueryExtensions
{
    public abstract class NumericFilterCondition : FilterCondition
    {
        protected NumericFilterCondition() { }

        /// <summary>
        /// Creates a new instance of <see cref="NumericFilterCondition"/>. Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="operation">Valid operation values are: equals, notEqual, lessThan, lessThanOrEqual, greaterThan, greaterThanOrEqual and inRange.<br/>
        /// If operation value isn't any of these values, "equals" value will be used.</param>
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

        /// <summary>
        /// Creates a new instance of <see cref="NumericFilterCondition"/>.
        /// </summary>
        /// <param name="property">The property name, case insensitive.</param>
        /// <param name="operation">The operation to compare.</param>
        protected NumericFilterCondition(string property, NumericOperations operation)
        {
            Property = property;
            Operation = operation;
        }

        protected NumericOperations Operation { get; set; }
    }
}
