using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int) Math.Ceiling(count / (double) pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        /// <summary>
        /// PageSize is the number many of items we 
        /// want to dispay per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// TotalCount is the total number of items are in our query.
        /// Let's say we want all our females users
        /// and we have 10 female users in our DB
        /// then TotalCount is 10
        /// </summary>
        public int TotalCount { get; set; }


        public static async Task<PaginatedList<T>> CreatePaginatedListAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = await source.CountAsync(); //..this will make a DB call

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
