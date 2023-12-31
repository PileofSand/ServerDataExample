using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FunCraftersTask.Data
{
    public class StaticLoadingStrategy : IDataLoadingStrategy
    {
        private List<DataItem> _allItems;
        private IDataServer _dataServer;
        private const int PageSize = 5;

        public StaticLoadingStrategy(IDataServer dataServer)
        {
            _dataServer = dataServer;
        }

        public Task LoadPageAsync(int pageIndex, CancellationToken ct)
        {
            // No additional loading needed for static strategy
            return Task.CompletedTask;
        }
        
        public async Task LoadDataAsync(CancellationToken ct)
        {
            int totalItems = await _dataServer.DataAvailable(ct);
            _allItems = await _dataServer.RequestData(0, totalItems, ct) as List<DataItem>;
        }

        public int GetTotalItems()
        {
            return _allItems.Count;
        }
        
        public IEnumerable<DataItem> GetPageItems(int pageIndex)
        {
            int skip = pageIndex * PageSize;
            return _allItems.Skip(skip).Take(PageSize);
        }
    }
}