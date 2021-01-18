using JSoft.QueryExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QueryExtensions.Test
{
    public class SortTests
    {
        List<Person> NullPeople;
        IQueryable<Person> NullPeopleQuery;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestQuerySort()
        {
            var people = Person.PeopleQuery().Sort("surname");
            Assert.AreEqual("Ashe", people.First().Surname);
            Assert.AreEqual("Spelvin", people.Last().Surname);
        }

        [Test]
        public void TestListSort()
        {
            var people = Person.People().Sort("Name");
            Assert.AreEqual("Allegra", people.First().Name);
            Assert.AreEqual("Ponsonby", people.Last().Name);
        }

        [Test]
        public void TestListSortBoolAscending()
        {
            var asc = Person.People().Sort("height", true);
            Assert.AreEqual(1.45, asc.First().Height);
            Assert.AreEqual(1.93, asc.Last().Height);
        }

        [Test]
        public void TestQuerySortBoolDescending()
        {
            var asc = Person.PeopleQuery().Sort("height", false);
            Assert.AreEqual(1.93, asc.First().Height);
            Assert.AreEqual(1.45, asc.Last().Height);
        }

        [Test]
        public void TestListSortStringAscending()
        {
            var asc = Person.People().Sort("height;asc");
            Assert.AreEqual(1.45, asc.First().Height);
            Assert.AreEqual(1.93, asc.Last().Height);
        }

        [Test]
        public void TestListSortStringDescending()
        {
            var desc = Person.People().Sort("id;desc");
            Assert.AreEqual(10, desc.First().Id);
            Assert.AreEqual(1, desc.Last().Id);
        }


        [Test]
        [TestCase("asgsdgr")]
        [TestCase("asfgve;")]
        [TestCase("--s;vee")]
        public void TestInvalidProperties(string orderstring)
        {
            var peoeple = Person.PeopleQuery().Sort(orderstring);
            Assert.AreEqual(1, peoeple.First().Id);
            Assert.AreEqual(10, peoeple.Last().Id);
        }

        [Test]
        public void TestNullSourcesPageSize()
        {
            Assert.Throws(typeof(ArgumentNullException), () => NullPeople.Sort("id;desc"));
            Assert.Throws(typeof(ArgumentNullException), () => NullPeopleQuery.Sort("id;desc"));
        }

    }
}