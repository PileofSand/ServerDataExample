using System;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using FunCraftersTask.Entities;
using FunCraftersTask.Utilities;
using UnityEngine.Serialization;

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

        public void DisplayItems(IEnumerable<DataItem> items)
        {
            var itemsToReturn = new List<ItemEntity>(_itemPool.ActiveItems);
            
            foreach (var item in itemsToReturn)
            {
                _itemPool.Return(item);
            }
            
            var itemList = items.ToList();
            for (int i = 0; i < itemList.Count; i++)
            {
                var itemEntity = _itemPool.Get();
                itemEntity.Setup(itemList[i], i);
            }
        }
    }
}