using System.Collections.Generic;
using UnityEngine;

namespace FunCraftersTask.Utilities
{
    public class GenericObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Stack<T> _pool = new Stack<T>();
        private readonly Transform _parent;
        private readonly List<T> _activeItems = new List<T>();

        public GenericObjectPool(T prefab, int initialCapacity, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialCapacity; i++)
            {
                CreateObject();
            }
        }

        public T Get()
        {
            T obj;
            if (_pool.Count == 0)
            {
                obj = CreateObject();
            }
            else
            {
                obj = _pool.Pop();
            }
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

        public IEnumerable<T> ActiveItems => _activeItems;
    }
}