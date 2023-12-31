using FunCraftersTask.Controllers;
using FunCraftersTask.Entities;
using FunCraftersTask.Models;
using FunCraftersTask.Data;
using FunCraftersTask.Utilities;
using FunCraftersTask.Views;
using UnityEngine;
using Zenject;

namespace FunCraftersTask.DependencyInjection
{
    public class GameInstaller : MonoInstaller
    {
        public ItemEntity itemPrefab;
        public Transform itemContainer;

        public override void InstallBindings()
        {
            Container.Bind<IDataServer>().To<DataServerMock>().AsSingle();
            Container.BindInterfacesAndSelfTo<SelectionModel>().AsSingle().NonLazy();
            Container.Bind<GenericObjectPool<ItemEntity>>().AsSingle()
                .WithArguments(itemPrefab, 5, itemContainer);
            Container.BindInterfacesAndSelfTo<SelectionController>().AsSingle().NonLazy();
            Container.Bind<ISelectionView>().FromComponentInHierarchy().AsSingle();
        }
    }
}