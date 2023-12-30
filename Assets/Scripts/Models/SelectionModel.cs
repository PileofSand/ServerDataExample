using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FunCraftersTask.Services.Server;
using UnityEngine;
using Zenject;

namespace FunCraftersTask.Models
{
    public class SelectionModel : IInitializable, IDisposable
    {
        private const int PageSize = 5;
        private CancellationTokenSource _cts;
        private IDataServer _dataServer;
        private List<DataItem> _items;
        private readonly Dictionary<int, List<DataItem>> _pageCache = new();
        public int CurrentPageIndex { get; private set; }
        public int TotalItems { get; private set; }

        public int TotalPages => TotalItems / PageSize + (TotalItems % PageSize > 0 ? 1 : 0);

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        public void Initialize()
        {
            _cts = new CancellationTokenSource();
        }

        [Inject]
        public void Construct(IDataServer dataServer)
        {
            _dataServer = dataServer;
        }

        public async Task InitializeAsync()
        {
            try
            {
                var totalItems = await _dataServer.DataAvailable(_cts.Token);
                SetTotalItems(totalItems);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing data: {e.Message}");
            }
        }

        public IEnumerable<DataItem> GetCurrentPageItems()
        {
            return _items;
        }

        public async Task LoadPage(int pageIndex)
        {
            try
            {
                await GetItemsAsync(pageIndex);
                SetPageIndex(pageIndex);
                PrefetchNextPage(pageIndex);
            }
            catch (Exception e)
            {
                //TODO: Add retry functionallity with maximum tries if fetching data fails. 
                Debug.LogError($"Error loading page {pageIndex}: {e.Message}");
            }
        }

        private async Task GetItemsAsync(int pageIndex)
        {
            if (!IsPageCached(pageIndex))
            {
                var itemsToRequest = GetItemsCountForPage(pageIndex);
                _items =
                    await _dataServer.RequestData(pageIndex * PageSize, itemsToRequest, _cts.Token) as List<DataItem>;
                CachePage(pageIndex, _items);
            }
            else
            {
                _items = GetCachedPage(pageIndex);
            }
        }

        private int GetItemsCountForPage(int pageIndex)
        {
            var totalRemainingItems = TotalItems - pageIndex * PageSize;
            return Math.Min(PageSize, totalRemainingItems);
        }

        private void PrefetchNextPage(int pageIndex)
        {
            //Fetching next page in advance, we could load all data at once on initialize but what if there is a lot of data in the future. Solution varies here.
            if ((pageIndex + 1) * PageSize < TotalItems && !IsPageCached(pageIndex + 1))
            {
                Task.Run(async () => await SafeGetItemsAsync(pageIndex + 1));
            }
        }
        
        private async Task SafeGetItemsAsync(int pageIndex)
        {
            try
            {
                await GetItemsAsync(pageIndex);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error prefetching page {pageIndex}: {e.Message}");
            }
        }

        private void SetTotalItems(int itemsNumber)
        {
            TotalItems = itemsNumber;
        }

        private void SetPageIndex(int index)
        {
            if (index >= 0 && index < TotalPages) CurrentPageIndex = index;
        }

        private bool IsPageCached(int pageIndex)
        {
            return _pageCache.ContainsKey(pageIndex);
        }

        private void CachePage(int pageIndex, List<DataItem> items)
        {
            _pageCache[pageIndex] = items;
        }

        private List<DataItem> GetCachedPage(int pageIndex)
        {
            return _pageCache.TryGetValue(pageIndex, out var items) ? items : null;
        }
    }
}