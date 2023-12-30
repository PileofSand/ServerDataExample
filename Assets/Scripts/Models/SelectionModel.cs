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
        private List<DataItem> _items;
        private IDataServer _dataServer;
        private Dictionary<int, List<DataItem>> _pageCache = new Dictionary<int, List<DataItem>>();
        private CancellationTokenSource _cts;
        private const int PageSize = 5;
        
        public int TotalPages => TotalItems / PageSize;
        public int CurrentPageIndex { get; private set; } = 0;
        public int TotalItems { get; private set; }

        [Inject]
        public void Construct(IDataServer dataServer)
        {
            _dataServer = dataServer;
        }
        
        public void Initialize()
        {
            _cts = new CancellationTokenSource();
            InitializeAsync();
        }
        
        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
        
        private async void InitializeAsync()
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
                Debug.LogError($"Error loading page {pageIndex}: {e.Message}");
            }
        }

        private async Task GetItemsAsync(int pageIndex)
        {
            if (!IsPageCached(pageIndex))
            {
                _items = await _dataServer.RequestData(pageIndex * PageSize, PageSize, _cts.Token) as List<DataItem>;
                CachePage(pageIndex, _items);
            }
            else
            {
                _items = GetCachedPage(pageIndex);
            }
        }

        private void PrefetchNextPage(int pageIndex)
        {
            if ((pageIndex + 1) * PageSize < TotalItems && !IsPageCached(pageIndex+1))
            {
                GetItemsAsync(pageIndex + 1);
            }
        }
        
        private void SetTotalItems(int itemsNumber)
        {
            TotalItems = itemsNumber;
        }
        
        private void SetPageIndex(int index)
        {
            if (index >= 0 && index < TotalPages)
            {
                CurrentPageIndex = index;
            }
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