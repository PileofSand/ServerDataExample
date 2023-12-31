using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FunCraftersTask.Data
{
     public class DynamicLoadingStrategy : IDataLoadingStrategy
    {
        private readonly IDataServer _dataServer;
        private readonly DataCache _cache;
        private const int PageSize = 5;
        private const int PrefetchPages = 2; // Number of pages to prefetch
        private int _totalItems;

        public DynamicLoadingStrategy(IDataServer dataServer)
        {
            _dataServer = dataServer;
            _cache = new DataCache();
        }

        public async Task LoadDataAsync(CancellationToken ct)
        {
            _totalItems = await _dataServer.DataAvailable(ct);
        }

        public async Task LoadPageAsync(int pageIndex, CancellationToken ct)
        {
            await EnsurePageLoaded(pageIndex, ct);
            PrefetchAdditionalPages(pageIndex, ct);
        }

        public int GetTotalItems()
        {
            return _totalItems;
        }

        public IEnumerable<DataItem> GetPageItems(int pageIndex)
        {
            return _cache.GetCachedPage(pageIndex) ?? Enumerable.Empty<DataItem>();
        }

        private async Task EnsurePageLoaded(int pageIndex, CancellationToken ct)
        {
            if (!_cache.IsPageCached(pageIndex))
            {
                await FetchAndCachePage(pageIndex, ct);
            }
        }

        private void PrefetchAdditionalPages(int currentPageIndex, CancellationToken ct)
        {
            for (int i = 1; i <= PrefetchPages; i++)
            {
                int prefetchPageIndex = currentPageIndex + i;
                if (IsPageWithinBounds(prefetchPageIndex) && !_cache.IsPageCached(prefetchPageIndex))
                {
                    Task.Run(() => FetchAndCachePage(prefetchPageIndex, ct));
                }
            }
        }

        private bool IsPageWithinBounds(int pageIndex)
        {
            return pageIndex * PageSize < _totalItems;
        }

        private async Task FetchAndCachePage(int pageIndex, CancellationToken ct)
        {
            var itemsToRequest = GetItemsCountForPage(pageIndex);
            var items = await _dataServer.RequestData(pageIndex * PageSize, itemsToRequest, ct) as List<DataItem>;
            _cache.CachePage(pageIndex, items);
        }

        private int GetItemsCountForPage(int pageIndex)
        {
            int totalRemainingItems = _totalItems - pageIndex * PageSize;
            return Math.Min(PageSize, totalRemainingItems);
        }
    }

    public class DataCache
    {
        private readonly Dictionary<int, List<DataItem>> _pageCache = new();

        public bool IsPageCached(int pageIndex)
        {
            return _pageCache.ContainsKey(pageIndex);
        }

        public List<DataItem> GetCachedPage(int pageIndex)
        {
            return _pageCache.TryGetValue(pageIndex, out var items) ? items : null;
        }

        public void CachePage(int pageIndex, List<DataItem> items)
        {
            _pageCache[pageIndex] = items;
        }
    }
}