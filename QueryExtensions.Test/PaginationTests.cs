using JSoft.QueryExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QueryExtensions.Test
{
    public class PaginationTests
    {
        List<int> Integers;
        List<int> NullList;
        IQueryable<int> IntQuery;
        IQueryable<int> NullQuery;
        const int upperLimit = 35;

        [SetUp]
        public void Setup()
        {
            Integers = Enumerable.Range(1, upperLimit).ToList();
            IntQuery = Integers.AsQueryable();
        }

        [Test]
        public async Task TestQueryToPagedListAsync()
        {
            var paged = await IntQuery.ToPagedListAsync(3, 10);
            Assert.AreEqual(21, paged.First());
            Assert.AreEqual(30, paged.Last());
            Assert.AreEqual(10, paged.Count);
            Assert.AreEqual(upperLimit, paged.TotalCount);
            Assert.AreEqual(10, paged.PageSize);
            Assert.AreEqual(3, paged.CurrentPage);
            Assert.AreEqual(4, paged.TotalPages);
            Assert.AreEqual(true, paged.HasNext);
            Assert.AreEqual(true, paged.HasPrevious);
        }

        [Test]
        public void TestQueryToPagedList()
        {
            var paged = IntQuery.ToPagedList(3, 10);
            Assert.AreEqual(21, paged.First());
            Assert.AreEqual(30, paged.Last());
            Assert.AreEqual(10, paged.Count);
            Assert.AreEqual(upperLimit, paged.TotalCount);
            Assert.AreEqual(10, paged.PageSize);
            Assert.AreEqual(3, paged.CurrentPage);
            Assert.AreEqual(4, paged.TotalPages);
            Assert.AreEqual(true, paged.HasNext);
            Assert.AreEqual(true, paged.HasPrevious);
        }

        [Test]
        public void TestListToPagedList()
        {
            var paged = Integers.ToPagedList(3, 10);
            Assert.AreEqual(21, paged.First());
            Assert.AreEqual(30, paged.Last());
            Assert.AreEqual(10, paged.Count);
            Assert.AreEqual(upperLimit, paged.TotalCount);
            Assert.AreEqual(10, paged.PageSize);
            Assert.AreEqual(3, paged.CurrentPage);
            Assert.AreEqual(4, paged.TotalPages);
            Assert.AreEqual(true, paged.HasNext);
            Assert.AreEqual(true, paged.HasPrevious);
        }

        [Test]
        public async Task TestQueryToPagedListAsyncProjection()
        {
            var paged = await IntQuery.ToPagedListAsync(3, 10, i => i + 1);
            Assert.AreEqual(22, paged.First());
            Assert.AreEqual(31, paged.Last());
            Assert.AreEqual(10, paged.Count);
            Assert.AreEqual(upperLimit, paged.TotalCount);
            Assert.AreEqual(10, paged.PageSize);
            Assert.AreEqual(3, paged.CurrentPage);
            Assert.AreEqual(4, paged.TotalPages);
            Assert.AreEqual(true, paged.HasNext);
            Assert.AreEqual(true, paged.HasPrevious);
        }

        [Test]
        public void TestListToPagedListProjection()
        {
            var paged = Integers.ToPagedList(3, 10, i => i + 1);
            Assert.AreEqual(22, paged.First());
            Assert.AreEqual(31, paged.Last());
            Assert.AreEqual(10, paged.Count);
            Assert.AreEqual(upperLimit, paged.TotalCount);
            Assert.AreEqual(10, paged.PageSize);
            Assert.AreEqual(3, paged.CurrentPage);
            Assert.AreEqual(4, paged.TotalPages);
            Assert.AreEqual(true, paged.HasNext);
            Assert.AreEqual(true, paged.HasPrevious);
        }

        [Test]
        public void TestLowerBounds()
        {
            var paged = Integers.ToPagedList(1, 5);
            Assert.AreEqual(1, paged.First());
            Assert.AreEqual(5, paged.Last());
            Assert.AreEqual(true, paged.HasNext);
            Assert.AreEqual(false, paged.HasPrevious);
        }

        [Test]
        public void TestUpperBounds()
        {
            var paged = Integers.ToPagedList(4, 10);
            Assert.AreEqual(31, paged.First());
            Assert.AreEqual(35, paged.Last());
            Assert.AreEqual(false, paged.HasNext);
            Assert.AreEqual(true, paged.HasPrevious);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void TestInvalidPageNumbers(int pageNumber)
        {
            Assert.Throws(typeof(ArgumentException), () => Integers.ToPagedList(pageNumber, 10));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        public void TestInvalidPageSize(int pageSize)
        {
            Assert.Throws(typeof(ArgumentException), () => Integers.ToPagedList(1, pageSize));
        }

        [Test]
        public void TestNullSourcesPageSize()
        {
            Assert.Throws(typeof(ArgumentNullException), () => NullList.ToPagedList(1, 10));
            Assert.Throws(typeof(ArgumentNullException), () => NullQuery.ToPagedList(1, 10));
        }

        [Test]
        [TestCase(5, 10)]
        public void TestOutsideUpperBounds(int pageNumber, int pageSize)
        {
            var paged = Integers.ToPagedList(pageNumber, pageSize);
            Assert.AreEqual(0, paged.Count);
            Assert.AreEqual(upperLimit, paged.TotalCount);
            Assert.AreEqual(pageSize, paged.PageSize);
            Assert.AreEqual(1, paged.CurrentPage);
            Assert.AreEqual(4, paged.TotalPages);
            Assert.AreEqual(true, paged.HasNext);
            Assert.AreEqual(false, paged.HasPrevious);
        }
    }
}