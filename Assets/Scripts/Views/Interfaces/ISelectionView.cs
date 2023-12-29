using System;
using System.Collections.Generic;

namespace FunCraftersTask.Views
{
    public interface ISelectionView
    {
        event Action<int> OnPageChanged;
        void DisplayItems(IList<DataItem> items);
        void SetNavigationButtons(bool showNext, bool showPrevious);
    }
}