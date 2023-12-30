using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using FunCraftersTask.Entities;
using FunCraftersTask.Utilities;

namespace FunCraftersTask.Views
{
    public class SelectionView : MonoBehaviour, ISelectionView
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private ItemEntity _itemPrefab;

        [SerializeField] private GenericObjectPool<ItemEntity> _itemPool;

        public IObservable<Unit> OnNextClicked => _nextButton.OnClickAsObservable();
        public IObservable<Unit> OnPreviousClicked => _previousButton.OnClickAsObservable();

        private void Awake()
        {
            _itemPool = new GenericObjectPool<ItemEntity>(_itemPrefab, 5, _itemContainer);
        }

        public void DisplayItems(IEnumerable<DataItem> items, int currentPage, int totalPages)
        {
            var itemList = items.ToList();
            var activeItemsList = _itemPool.ActiveItems.ToList();
            var activeItemCount = activeItemsList.Count;
            
            for (int i = 0; i < itemList.Count; i++)
            {
                ItemEntity itemEntity;
                if (i < activeItemCount)
                {
                    itemEntity = activeItemsList[i];
                }
                else
                {
                    itemEntity = _itemPool.Get();
                }
                itemEntity.Setup(itemList[i], i);
            }
            
            for (int i = itemList.Count; i < activeItemCount; i++)
            {
                _itemPool.Return(activeItemsList[i]);
            }

            _previousButton.interactable = currentPage > 0;
            _nextButton.interactable = currentPage < totalPages - 1;
        }
    }
}