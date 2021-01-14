using JSoft.QueryExtensions;
using NUnit.Framework;

namespace QueryExtensions.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            //var filter = AgGridFilterParser.Parse(Properties.Resources.SimpleFilter);
            //var filter2 = AgGridFilterParser.Parse(Properties.Resources.ComplexFilter);
            var filter3 = AgGridFilterParser.Parse(Properties.Resources.DateSimpleNumberRange);
            var filter4 = AgGridFilterParser.Parse(Properties.Resources.DateRange);
        }
    }
}