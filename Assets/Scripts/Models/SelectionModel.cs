using System.Collections.Generic;
using System.Linq;

namespace FunCraftersTask.Models
{
    public class SelectionModel
    {
        private List<DataItem> _items;
        private const int PageSize = 5;
        private int TotalPages => (TotalItems + PageSize - 1) / PageSize;
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