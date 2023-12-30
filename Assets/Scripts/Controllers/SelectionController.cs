using UniRx;
using Zenject;
using System;
using FunCraftersTask.Models;
using FunCraftersTask.Views;
using UnityEngine;

namespace FunCraftersTask.Controllers
{
    public class SelectionController : IInitializable
    {
        private ISelectionView _view;
        private SelectionModel _model;
        private const int PageSize = 5;
        private int _totalItems;

        [Inject]
        public void Construct(ISelectionView view, SelectionModel model)
        {
            _view = view;
            _model = model;
        }

        public void Initialize()
        {
            _view.OnNextClicked.Subscribe(_ => LoadNextPage());
            _view.OnPreviousClicked.Subscribe(_ => LoadPreviousPage());

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                await _model.InitializeAsync();
                await _model.LoadPage(0);
                UpdateView();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error initializing data: {e.Message}");
            }
        }

        private async void LoadNextPage()
        {
            if ((_model.CurrentPageIndex + 1) * PageSize < _model.TotalItems)
            {
                await _model.LoadPage(_model.CurrentPageIndex + 1);
                UpdateView();
            }
        }

        private async void LoadPreviousPage()
        {
            if (_model.CurrentPageIndex > 0)
            {
                await _model.LoadPage(_model.CurrentPageIndex - 1);
                UpdateView();
            }
        }

        private void UpdateView()
        {
            _view.DisplayItems(_model.GetCurrentPageItems(),_model.CurrentPageIndex, _model.TotalPages);
        }
    }
}
