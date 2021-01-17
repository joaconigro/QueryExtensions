using System;
using System.Collections.Generic;

namespace JSoft.QueryExtensions
{
    /// <summary>
    /// Defines the <see cref="PagedList{T}" />.
    /// </summary>
    /// <typeparam name="T">.</typeparam>
    public class PagedList<T> : List<T>
    {
        /// <summary>
        /// Gets the current page of the list.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// Gets the total number of pages of the list.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Gets the page size of the list.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// Gets the total items count of the original list.
        /// </summary>
        public long TotalCount { get; private set; }

        /// <summary>
        /// Returns true if isn't the first page.
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;

        /// <summary>
        /// Returns true if isn't the last page.
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}"/> class, using a <see cref="IEnumerable{T}"/> of elements, <br/>
        /// the original items count, the current page number and the page size.
        /// </summary>
        /// <param name="items">The elements that will contain.</param>
        /// <param name="count">The original items count.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        public PagedList(IEnumerable<T> items, long count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            CurrentPage = pageNumber > TotalPages ? 1 : pageNumber;

            AddRange(items);
        }
    }
}
