using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunCraftersTask.Data;
using UnityEngine;
using Zenject;

namespace FunCraftersTask.Models
{
    public class SelectionModel : IInitializable, IDisposable
    {
        private IDataLoadingStrategy _loadingStrategy;
        private CancellationTokenSource _cts;
        private const int PageSize = 5;
        private List<DataItem> _items;
        public int CurrentPageIndex { get; private set; }
        public int TotalItems { get; private set; }
        
        public int TotalPages
        {
            get
            {
                int totalItems = _loadingStrategy.GetTotalItems();
                if (totalItems == 0)
                {
                    return 0;
                }

                int fullPages = totalItems / PageSize;
                bool hasPartialPage = totalItems % PageSize > 0;

                return fullPages + (hasPartialPage ? 1 : 0);
            }
        }
        
        
        [Inject]
        public void Construct(IDataServer dataServer)
        {
            // The loading strategy is chosen based on the desired data loading behavior.
            // DynamicLoadingStrategy: Loads data on-demand and prefetches next pages for smoother transitions.
            // StaticLoadingStrategy: Loads all data at once, suitable for scenarios with a relatively small dataset
            // that can be fetched and held in memory without significant performance concerns.
            // Uncomment the desired strategy based on the application's requirements and data characteristics.
            
            //_loadingStrategy = new DynamicLoadingStrategy(dataServer);
            _loadingStrategy = new StaticLoadingStrategy(dataServer);
        }

        public void Initialize()
        {
            _cts = new CancellationTokenSource();
        }

        public async Task InitializeAsync()
        {
            try
            {
                await _loadingStrategy.LoadDataAsync(_cts.Token);
                TotalItems = _loadingStrategy.GetTotalItems();
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
                await _loadingStrategy.LoadPageAsync(pageIndex, _cts.Token);
                _items = _loadingStrategy.GetPageItems(pageIndex).ToList();
                SetPageIndex(pageIndex);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading page {pageIndex}: {e.Message}");
            }
        }

        private void SetPageIndex(int index)
        {
            if (index >= 0 && index < TotalPages)
            {
                CurrentPageIndex = index;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
