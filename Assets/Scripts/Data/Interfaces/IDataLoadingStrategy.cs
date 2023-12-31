using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FunCraftersTask.Data
{
    public interface IDataLoadingStrategy
    {
        Task LoadDataAsync(CancellationToken ct);
        IEnumerable<DataItem> GetPageItems(int pageIndex);
        Task LoadPageAsync(int pageIndex, CancellationToken ct);
        int GetTotalItems();
    }
}