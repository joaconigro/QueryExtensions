using JSoft.QueryExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryExtensions.Test
{
    public class FilterTests
    {
        const string StringContains = "{\"name\":{\"filterType\":\"text\",\"type\":\"contains\",\"filter\":\"o\"}}";
        const string StringNotContains = "{\"name\":{\"filterType\":\"text\",\"type\":\"notContains\",\"filter\":\"o\"}}";
        const string StringEqual = "{\"name\":{\"filterType\":\"text\",\"type\":\"equals\",\"filter\":\"John\"}}";
        const string StringNotEquals = "{\"name\":{\"filterType\":\"text\",\"type\":\"notEqual\",\"filter\":\"John\"}}";
        const string StringStartsWith = "{\"name\":{\"filterType\":\"text\",\"type\":\"startsWith\",\"filter\":\"j\"}}";
        const string StringEndsWith = "{\"name\":{\"filterType\":\"text\",\"type\":\"endsWith\",\"filter\":\"r\"}}";
        const string StringAnd = "{\"surname\":{\"filterType\":\"text\",\"operator\":\"AND\",\"condition1\":{\"filterType\":\"text\",\"type\":\"contains\",\"filter\":\"e\"},\"condition2\":{\"filterType\":\"text\",\"type\":\"endsWith\",\"filter\":\"n\"}}}";
        const string StringOr = "{\"surname\":{\"filterType\":\"text\",\"operator\":\"OR\",\"condition1\":{\"filterType\":\"text\",\"type\":\"contains\",\"filter\":\"e\"},\"condition2\":{\"filterType\":\"text\",\"type\":\"endsWith\",\"filter\":\"n\"}}}";

        const string NumberEquals = "{\"id\":{\"filterType\":\"number\",\"type\":\"equals\",\"filter\":3}}";
        const string NumberNotEquals = "{\"id\":{\"filterType\":\"number\",\"type\":\"notEqual\",\"filter\":3}}";
        const string NumberLessThan = "{\"id\":{\"filterType\":\"number\",\"type\":\"lessThan\",\"filter\":3}}";
        const string NumberLessThanOrEqual = "{\"id\":{\"filterType\":\"number\",\"type\":\"lessThanOrEqual\",\"filter\":3}}";
        const string NumberGreaterThan = "{\"height\":{\"filterType\":\"number\",\"type\":\"greaterThan\",\"filter\":1.73}}";
        const string NumberGreaterThanOrEqual = "{\"height\":{\"filterType\":\"number\",\"type\":\"greaterThanOrEqual\",\"filter\":1.73}}";
        const string NumberInRange = "{\"height\":{\"filterType\":\"number\",\"type\":\"inRange\",\"filter\":1.7,\"filterTo\":1.8}}";
        const string NumberInRangeOrGreater = "{\"height\":{\"filterType\":\"number\",\"operator\":\"OR\",\"condition1\":{\"filterType\":\"number\",\"type\":\"inRange\",\"filter\":1.5,\"filterTo\":1.7},\"condition2\":{\"filterType\":\"number\",\"type\":\"greaterThan\",\"filter\":1.9}}}";

        const string DateEquals = "";
        const string DateNotEquals = "";
        const string DateLessThan = "";
        const string DateLessThanOrEqual = "";
        const string DateGreaterThan = "";
        const string DateGreaterThanOrEqual = "";
        const string DateInRange = "";

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        [TestCase(NumberEquals, 1)]
        [TestCase(NumberNotEquals, 9)]
        [TestCase(NumberLessThan, 2)]
        [TestCase(NumberLessThanOrEqual, 3)]
        [TestCase(NumberGreaterThan, 5)]
        [TestCase(NumberGreaterThanOrEqual, 6)]
        [TestCase(NumberInRange, 2)]
        [TestCase(NumberInRangeOrGreater, 4)]
        public void NumberFilterTests(string value, int expected)
        {
            var filters = AgGridFilterParser.Parse(value);
            var people = Person.People().Filter(filters);
            Assert.AreEqual(expected, people.Count());
        }

        [Test]
        [TestCase(StringContains, 5)]
        [TestCase(StringNotContains, 5)]
        [TestCase(StringEqual, 1)]
        [TestCase(StringNotEquals, 9)]
        [TestCase(StringStartsWith, 2)]
        [TestCase(StringEndsWith, 2)]
        [TestCase(StringAnd, 2)]
        [TestCase(StringOr, 5)]
        public void StringFilterTests(string value, int expected)
        {
            var filters = AgGridFilterParser.Parse(value);
            var people = Person.People().Filter(filters);
            Assert.AreEqual(expected, people.Count());
        }
    }
}