using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FunCraftersTask.Entities;
using FunCraftersTask.Utilities;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace FunCraftersTask.Views
{
    public class SelectionView : MonoBehaviour, ISelectionView
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private ItemEntity _itemPrefab;
        [SerializeField] private GameObject _loadingIcon;

        private GenericObjectPool<ItemEntity> _itemPool;

        private void Awake()
        {
            _itemPool = new GenericObjectPool<ItemEntity>(_itemPrefab, 5, _itemContainer);
        }

        public IObservable<Unit> OnNextClicked => _nextButton.OnClickAsObservable();
        public IObservable<Unit> OnPreviousClicked => _previousButton.OnClickAsObservable();

        public void DisplayItems(IEnumerable<DataItem> items, int currentPage, int totalPages)
        {
            var itemList = items.ToList();
            var activeItemsList = _itemPool.ActiveItems.ToList();
            var activeItemCount = activeItemsList.Count;

            for (var i = 0; i < itemList.Count; i++)
            {
                ItemEntity itemEntity;
                if (i < activeItemCount)
                    itemEntity = activeItemsList[i];
                else
                    itemEntity = _itemPool.Get();
                itemEntity.Setup(itemList[i], i);
            }

            for (var i = itemList.Count; i < activeItemCount; i++) _itemPool.Return(activeItemsList[i]);

            _previousButton.interactable = currentPage > 0;
            _nextButton.interactable = currentPage < totalPages - 1;
        }

        public void SetLoadingIconActive(bool isActive)
        {
            _loadingIcon.SetActive(isActive);
            if (isActive)
                _loadingIcon.transform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360)
                    .SetLoops(-1, LoopType.Incremental)
                    .SetEase(Ease.Linear);
            else
                DOTween.Kill(_loadingIcon.transform);
        }
    }
}