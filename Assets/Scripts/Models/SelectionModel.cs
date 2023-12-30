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
        private readonly IDataServer _dataServer;
        private Dictionary<int, List<DataItem>> _pageCache = new Dictionary<int, List<DataItem>>();
        private CancellationTokenSource _cts;
        private const int PageSize = 5;
        
        public int TotalPages => TotalItems / PageSize;
        public int CurrentPageIndex { get; private set; } = 0;
        public int TotalItems { get; private set; }

        public SelectionModel(IDataServer dataServer)
        {
            _dataServer = dataServer;
        }
        
        public void Initialize()
        {
            InitializeAsync();
            _cts = new CancellationTokenSource();
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
        
        public void SetTotalItems(int itemsNumber)
        {
            TotalItems = itemsNumber;
        }

        public async Task LoadPage(int pageIndex)
        {
            try
            {
                List<DataItem> items;
                if (!IsPageCached(pageIndex))
                {
                    items = await _dataServer.RequestData(pageIndex * PageSize, PageSize, _cts.Token) as List<DataItem>;
                    CachePage(pageIndex, items);
                }
                else
                {
                    items = GetCachedPage(pageIndex);
                }

                SetItems(items);
                SetPageIndex(pageIndex);
                PrefetchNextPage(pageIndex);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading page {pageIndex}: {e.Message}");
            }
        }

        private void PrefetchNextPage(int pageIndex)
        {
            if ((pageIndex + 1) * PageSize < TotalItems)
            {
                LoadPage(pageIndex + 1);
            }
        }
        
        private void SetPageIndex(int index)
        {
            if (index >= 0 && index < TotalPages)
            {
                CurrentPageIndex = index;
            }
        }

        private void SetItems(List<DataItem> items)
        {
            _items = items;
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