using System;
using System.Threading.Tasks;
using FunCraftersTask.Models;
using FunCraftersTask.Views;
using UniRx;
using UnityEngine;
using Zenject;

namespace FunCraftersTask.Controllers
{
    public class SelectionController : IInitializable
    {
        private const int PageSize = 5;
        private SelectionModel _model;
        private int _totalItems;
        private ISelectionView _view;

        public void Initialize()
        {
            _view.OnNextClicked.Subscribe(_ => LoadNextPage());
            _view.OnPreviousClicked.Subscribe(_ => LoadPreviousPage());

            InitializeAsync();
        }

        [Inject]
        public void Construct(ISelectionView view, SelectionModel model)
        {
            _view = view;
            _model = model;
        }

        private async void InitializeAsync()
        {
            try
            {
                await _model.InitializeAsync();
                await LoadPage(0);
                UpdateView();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing first page and view: {e.Message}");
            }
        }

        private async void LoadNextPage()
        {
            if ((_model.CurrentPageIndex + 1) * PageSize < _model.TotalItems)
                await LoadPage(_model.CurrentPageIndex + 1);
        }

        private async void LoadPreviousPage()
        {
            if (_model.CurrentPageIndex > 0) await LoadPage(_model.CurrentPageIndex - 1);
        }

        private async Task LoadPage(int pageIndex)
        {
            try
            {
                _view.SetLoadingIconActive(true);
                await _model.LoadPage(pageIndex);
                UpdateView();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading page {pageIndex}: {e.Message}");
            }
            finally
            {
                _view.SetLoadingIconActive(false);
            }
        }

        private void UpdateView()
        {
            _view.DisplayItems(_model.GetCurrentPageItems(), _model.CurrentPageIndex, _model.TotalPages);
        }
    }
}