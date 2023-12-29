using System.Threading;
using FunCraftersTask.Views;

public class SelectionPresenter
{
    private readonly ISelectionView _view;
    private readonly IDataServer _dataServer;
    private int _currentIndex = 0;

    public SelectionPresenter(ISelectionView view, IDataServer dataServer)
    {
        _view = view;
        _dataServer = dataServer;
        _view.OnPageChanged += HandlePageChanged;
        UpdateView();
    }

    private async void UpdateView()
    {
        var items = await _dataServer.RequestData(_currentIndex, 5, CancellationToken.None);
        _view.DisplayItems(items);
        // Update navigation buttons
    }

    private void HandlePageChanged(int newIndex)
    {
        _currentIndex = newIndex;
        UpdateView();
    }
}