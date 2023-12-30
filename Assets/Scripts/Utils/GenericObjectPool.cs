using System.Collections.Generic;
using UnityEngine;

namespace FunCraftersTask.Utilities
{
    public class GenericObjectPool<T> where T : Component
    {
        private readonly List<T> _activeItems = new();
        private readonly Transform _parent;
        private readonly Stack<T> _pool = new();
        private readonly T _prefab;

        public GenericObjectPool(T prefab, int initialCapacity, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;

            for (var i = 0; i < initialCapacity; i++) CreateObject();
        }

        public IEnumerable<T> ActiveItems => _activeItems;

        public T Get()
        {
            T obj;
            if (_pool.Count == 0)
                obj = CreateObject();
            else
                obj = _pool.Pop();
            obj.gameObject.SetActive(true);
            _activeItems.Add(obj);
            return obj;
        }

        public void Return(T objectToReturn)
        {
            objectToReturn.gameObject.SetActive(false);
            _pool.Push(objectToReturn);
            _activeItems.Remove(objectToReturn);
        }

        private T CreateObject()
        {
            var createdObject = Object.Instantiate(_prefab, _parent);
            createdObject.gameObject.SetActive(false);
            _pool.Push(createdObject);
            return createdObject;
        }
    }
}