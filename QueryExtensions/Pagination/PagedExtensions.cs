using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSoft.QueryExtensions
{
    public static class PagedExtensions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = await source.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static async Task<PagedList<TResult>> ToPagedListAsync<TSource, TResult>(this IQueryable<TSource> source, int pageNumber, int pageSize, Func<TSource, TResult> selector)
        {
            var count = source.Count();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = await source.Skip(skip).Take(pageSize).ToListAsync();

            return new PagedList<TResult>(items.Select(selector).ToList(), count, pageNumber, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = source.Skip(skip).Take(pageSize).ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static PagedList<TResult> ToPagedList<TSource, TResult>(this IEnumerable<TSource> source, int pageNumber, int pageSize, Func<TSource, TResult> selector)
        {
            var count = source.Count();
            var skip = CalculateSkip(count, pageNumber, pageSize);
            var items = source.Skip(skip).Take(pageSize).ToList();

            return new PagedList<TResult>(items.Select(selector).ToList(), count, pageNumber, pageSize);
        }

        static int CalculateSkip(int count, int pageNumber, int pageSize)
        {
            var skip = (pageNumber - 1) * pageSize;
            if (count > skip)
            {
                return skip;
            }
            return 0;
        }
    }
}
