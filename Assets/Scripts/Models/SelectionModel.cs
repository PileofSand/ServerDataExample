using System.Collections.Generic;
using System.Linq;

namespace FunCraftersTask.Models
{
    public class SelectionModel
    {
        private List<DataItem> _items;
        private Dictionary<int, List<DataItem>> _pageCache = new Dictionary<int, List<DataItem>>();
        private const int PageSize = 5;
        
        public int TotalPages => TotalItems / PageSize;
        public int CurrentPageIndex { get; private set; } = 0;
        public int TotalItems { get; private set; }

        public SelectionModel(List<DataItem> dataItems)
        {
            _items = dataItems;
        }

        public IEnumerable<DataItem> GetCurrentPageItems()
        {
            return _items;
        }

        public void SetPageIndex(int index)
        {
            if (index >= 0 && index < TotalPages)
            {
                CurrentPageIndex = index;
            }
        }
        
        public void CachePage(int pageIndex, List<DataItem> items)
        {
            _pageCache[pageIndex] = items;
        }
        
        public List<DataItem> GetCachedPage(int pageIndex)
        {
            return _pageCache.TryGetValue(pageIndex, out var items) ? items : null;
        }
        
        public void SetItems(List<DataItem> items)
        {
            _items = items;
        }

        public void SetTotalItems(int itemsNumber)
        {
            TotalItems = itemsNumber;
        }
    }
}