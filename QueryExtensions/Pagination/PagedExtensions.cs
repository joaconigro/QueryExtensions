using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSoft.QueryExtensions
{
    /// <summary>
    /// Extension methods for paging <see cref="IQueryable{T}"/> and <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class PagedExtensions
    {
        /// <summary>
        /// Returns a <see cref="PagedList{T}"/> from an <see cref="IQueryable{T}"/> source, using a page number and a page size. <br/>
        /// Usage: <code>var p = await source.ToPagedListAsync(1, 20);</code>
        /// </summary>
        /// <param name="source">The <see cref="IQueryable{T}"/> source.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The <see cref="Task{PagedList{T}}"/>.</returns>
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            CheckNullSource(source);
            var count = source.LongCount();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = await source.Skip(skip).Take(pageSize).ToListAsyncSafe();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Returns a <see cref="PagedList{T}"/> from an <see cref="IQueryable{T}"/> source, using a page number, a page size and a<br/>
        /// selector function to transform the elements of the source.
        /// Usage: <code>var p = await source.ToPagedListAsync(1, 20, o => o.InvokeSomeMethodOrProperty);</code>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source.</typeparam>
        /// <typeparam name="TResult">The type of the elements that will return after transform they.</typeparam>
        /// <param name="source">The <see cref="IQueryable{T}"/> source.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="selector">The selector <see cref="Func{TSource, TResult}"/> that will be invoke to transform the source items.</param>
        /// <returns>The <see cref="Task{PagedList{TResult}}"/>.</returns>
        public static async Task<PagedList<TResult>> ToPagedListAsync<TSource, TResult>(this IQueryable<TSource> source, int pageNumber, int pageSize, Func<TSource, TResult> selector)
        {
            CheckNullSource(source);
            var count = source.LongCount();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = await source.Skip(skip).Take(pageSize).ToListAsyncSafe();

            return new PagedList<TResult>(items.Select(selector), count, pageNumber, pageSize);
        }

        /// <summary>
        /// Returns a <see cref="PagedList{T}"/> from an <see cref="IEnumerable{T}"/> source, using a page number and a page size. <br/>
        /// Usage: <code>var p = source.ToPagedList(1, 20);</code>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> source.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The <see cref="PagedList{T}"/>.</returns>
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            CheckNullSource(source);
            var count = source.LongCount();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = source.Skip(skip).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        /// <summary>
        /// Returns a <see cref="PagedList{T}"/> from an <see cref="IEnumerable{T}"/> source, using a page number, a page size and a<br/>
        /// selector function to transform the elements of the source.
        /// Usage: <code>var p = source.ToPagedList(1, 20, o => o.InvokeSomeMethodOrProperty);</code>
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the source.</typeparam>
        /// <typeparam name="TResult">The type of the elements that will return after transform they.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> source.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="selector">The selector <see cref="Func{TSource, TResult}"/> that will be invoke to transform the source items.</param>
        /// <returns>The <see cref="PagedList{TResult}"/>.</returns>
        public static PagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source, int pageNumber, int pageSize, Func<TSource, TResult> selector)
        {
            CheckNullSource(source);
            var count = source.LongCount();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = source.Skip(skip).Take(pageSize).ToList();

            return new PagedList<TResult>(items.Select(selector), count, pageNumber, pageSize);
        }

        static void CheckNullSource<TSource>(IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "can't be null");
            }
        }

        static void CheckNullSource<TSource>(IQueryable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "can't be null");
            }
        }

        /// <summary>
        /// A safe way to calculate the skip count, to keep it within the source bounds.
        /// </summary>
        /// <param name="count">The total source count.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The <see cref="int"/>.</returns>
        static int CalculateSkip(long count, int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                throw new ArgumentException("Both, pageNumber and pageSize must be greater than 0");
            }

            var skip = (pageNumber - 1) * pageSize;
            if (count > skip)
            {
                return skip;
            }
            return (int)count;
        }
    }
}
