using System.Collections.Generic;
using UnityEngine;

namespace AOTVBR
{
    public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : Component
    {
        private Queue<T> pool;
        private Transform parent;
        [SerializeField]
        private T prefabToPool = default;
        [SerializeField]
        private int poolSize = 25;

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            pool = new Queue<T>(poolSize);
            parent = new GameObject($"{typeof(T).Name} Pool Objects").transform;
            DontDestroyOnLoad(parent);
            for (int i = 0; i < poolSize; i++)
            {
                AddNewObjectToPool();
            }
        }

        /// <summary>
        /// Get a object from the pool.
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T peekedObject = pool.Peek();
            CheckObjectInUse(peekedObject);

            T poppedObject = pool.Dequeue();
            SetObjectActiveState(poppedObject, true);

            return poppedObject;
        }

        /// <summary>
        /// Return a object to the pool.
        /// </summary>
        /// <param name="value"></param>
        public void Return(T value)
        {
            SetObjectActiveState(value, false);
            pool.Enqueue(value);
        }

        private void CheckObjectInUse(T peekedObject)
        {
            if (peekedObject == null)
            {
                AddNewObjectToPool();
            }
            else if (peekedObject.gameObject.activeSelf)
            {
                AddNewObjectToPool();
            }
        }

        private void AddNewObjectToPool()
        {
            T newObject = Instantiate(prefabToPool);
            SetObjectActiveState(newObject, false);
            newObject.transform.SetParent(parent);
            pool.Enqueue(newObject);
        }

        private void SetObjectActiveState(T poolObject, bool active)
            => poolObject.gameObject.SetActive(active);
    }
}