using Microsoft.EntityFrameworkCore;

namespace GameManagement.Shared.Helpers
{
    public class PagedList<T> : List<T>
    {
        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }

        public int CurrentPage { get; private set; }
        public bool HasNext => CurrentPage < TotalPages;
        public bool HasPrevious => CurrentPage > 1;
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            ArgumentNullException.ThrowIfNull(source);
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<IEnumerable<T>> source, int pageNumber, int pageSize)
        {
            ArgumentNullException.ThrowIfNull(source);
            var count = await source.CountAsync();
            var items = await source.SelectMany(x=>x).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}