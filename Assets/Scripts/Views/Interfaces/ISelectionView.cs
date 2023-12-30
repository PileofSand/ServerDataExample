using System;
using System.Collections.Generic;
using UniRx;

namespace FunCraftersTask.Views
{
    public interface ISelectionView
    {
        IObservable<Unit> OnNextClicked { get; }
        IObservable<Unit> OnPreviousClicked { get; }
        void DisplayItems(IEnumerable<DataItem> items, int currentPage, int totalPages);
        void SetLoadingIconActive(bool isActive);
    }
}