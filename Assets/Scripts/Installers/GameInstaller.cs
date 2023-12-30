using Zenject;
using FunCraftersTask.Services.Server;
using FunCraftersTask.Views;
using FunCraftersTask.Controllers;
using FunCraftersTask.Entities;
using FunCraftersTask.Models;
using FunCraftersTask.Utilities;
using UnityEngine;

public class GameInstaller : MonoInstaller
{
    public ItemEntity itemPrefab;
    public Transform itemContainer;

    public override void InstallBindings()
    {
        Container.Bind<IDataServer>().To<DataServerMock>().AsSingle();
        Container.Bind<ISelectionView>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GenericObjectPool<ItemEntity>>().AsSingle()
            .WithArguments(itemPrefab, 5, itemContainer);
        Container.BindInterfacesAndSelfTo<SelectionController>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SelectionModel>().AsSingle().NonLazy();;
    }
}