using UniRx;
using Zenject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FunCraftersTask.Models;
using FunCraftersTask.Services.Server;
using FunCraftersTask.Views;
using UnityEngine;

namespace FunCraftersTask.Controllers
{
    public class SelectionController : IInitializable, IDisposable
    {
        private ISelectionView _view;
        private SelectionModel _model;
        private IDataServer _dataServer;
        private const int PageSize = 5;
        private int _totalItems;
        private CancellationTokenSource _cts;

        [Inject]
        public void Construct(ISelectionView view, IDataServer dataServer)
        {
            _view = view;
            _dataServer = dataServer;
            _cts = new CancellationTokenSource();
        }

        public void Initialize()
        {
            _view.OnNextClicked.Subscribe(_ => LoadNextPage());
            _view.OnPreviousClicked.Subscribe(_ => LoadPreviousPage());
            _model = new SelectionModel(new List<DataItem>()); 
            
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                _totalItems = await _dataServer.DataAvailable(_cts.Token);
                _model.SetTotalItems(_totalItems);
                await LoadPage(0);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing data: {e.Message}");
            }
        }

        private async void LoadNextPage()
        {
            if ((_model.CurrentPageIndex + 1) * PageSize < _totalItems)
            {
                await LoadPage(_model.CurrentPageIndex + 1);
            }
        }

        private async void LoadPreviousPage()
        {
            if (_model.CurrentPageIndex > 0)
            {
                await LoadPage(_model.CurrentPageIndex - 1);
            }
        }

        private async Task LoadPage(int pageIndex)
        {
            try
            {
                var items = await _dataServer.RequestData(pageIndex * PageSize, PageSize, CancellationToken.None);
                _model.SetItems(items.ToList());
                _model.SetPageIndex(pageIndex);
                UpdateView();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading page {pageIndex}: {e.Message}");
            }
        }

        private void UpdateView()
        {
            _view.DisplayItems(_model.GetCurrentPageItems());
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            _model = null;
        }
    }
}
