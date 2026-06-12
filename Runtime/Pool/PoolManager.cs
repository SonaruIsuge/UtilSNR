using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using UtilSNR.Common;

namespace UtilSNR.Pool
{
    /// <summary>
    /// Manages object pools for different prefabs. 
    /// It allows spawning and despawning of objects, 
    /// as well as creating and warming up pools.
    /// </summary>
    public class PoolManager : TSingletonBehaviour<PoolManager>
    {
        private const int DEFAULT_CAPACITY = 10;
        private const int MAX_SIZE = 100;

        private Dictionary<Component, IObjectPool<Component>> pools = new();


        /// <summary>
        /// Spawn an object from the pool.
        /// </summary>
        public T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null, bool remainChildScale = false) where T : Component
        {
            if (prefab == null)
            {
                Debug.LogError("PoolManager: Prefab is null.");
                return null;
            }

            if (!pools.ContainsKey(prefab))
            {
                CreatePool(prefab);
            }

            var instance = pools[prefab].Get() as T;

            if (instance == null)
            {
                Debug.LogError($"PoolManager: Failed to spawn instance of {prefab.name}.");
                return null;
            }

            instance.transform.SetParent(parent);
            instance.transform.SetPositionAndRotation(position, rotation);

            // Ensure local scale matches prefab to avoid scaling issues from parent
            if (!remainChildScale || parent == null)
                instance.transform.localScale = prefab.transform.localScale;

            else
            {
                var prefabWorldScale = prefab.transform.lossyScale;
                var parentLossyScale = parent.lossyScale;
                instance.transform.localScale = new Vector3(
                    prefabWorldScale.x / parentLossyScale.x,
                    prefabWorldScale.y / parentLossyScale.y,
                    prefabWorldScale.z / parentLossyScale.z
                );
            }

            if (instance.TryGetComponent<IPoolable>(out var pooledObject))
            {
                pooledObject.OnSpawn();
            }

            return instance;
        }

        public T Spawn<T>(T prefab, Vector3 position, Transform parent = null, bool remainChildScale = false) where T : Component
        {
            return Spawn(prefab, position, Quaternion.identity, parent, remainChildScale);
        }

        public T Spawn<T>(T prefab, Transform parent = null, bool remainChildScale = false) where T : Component
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity, parent, remainChildScale);
        }

        /// <summary>
        /// Return an object to its pool.
        /// </summary>
        public void Despawn(Component component)
        {
            if (component == null)
                return;

            Despawn(component.gameObject);
        }

        /// <summary>
        /// Return a GameObject to its pool.
        /// </summary>
        public void Despawn(GameObject instance)
        {
            if (instance == null)
                return;

            if (instance.TryGetComponent<IPoolable>(out var pooledObject))
            {
                pooledObject.OnDespawn();

                // Set parent before deactivating to keep it clean
                if (pooledObject.instance != null)
                    pooledObject.instance.transform.SetParent(transform);

                pooledObject.ReturnToPool();
            }
            else
            {
                Debug.LogWarning($"PoolManager: Object {instance.name} doesn't have an IPoolable component. Destroying instead.");
                Destroy(instance);
            }
        }

        /// <summary>
        /// Create a pool for a specific prefab with custom settings.
        /// </summary>
        public void CreatePool<T>(T prefab, int defaultCapacity = DEFAULT_CAPACITY, int maxSize = MAX_SIZE) where T : Component
        {
            if (prefab == null) return;
            if (pools.ContainsKey(prefab)) return;

            var pool = new ObjectPool<Component>(
                createFunc: () => CreatePooledItem(prefab),
                actionOnGet: (obj) => obj.gameObject.SetActive(true),
                actionOnRelease: (obj) => obj.gameObject.SetActive(false),
                actionOnDestroy: (obj) => { if (obj != null) Destroy(obj.gameObject); },
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );

            pools.Add(prefab, pool);
        }

        /// <summary>
        /// Pre-instantiate a number of objects in the pool to avoid runtime spikes.
        /// </summary>
        public void WarmUp<T>(T prefab, int count) where T : Component
        {
            if (prefab == null || count <= 0) return;
            
            if (!pools.ContainsKey(prefab))
                CreatePool(prefab, count);

            List<T> tempInstances = new List<T>(count);
            for (int i = 0; i < count; i++)
            {
                tempInstances.Add(Spawn(prefab));
            }

            foreach (var instance in tempInstances)
            {
                Despawn(instance);
            }
        }

        private T CreatePooledItem<T>(T prefab) where T : Component
        {
            var instance = Instantiate(prefab);
            instance.name = prefab.name;

            if (!instance.TryGetComponent<IPoolable>(out var pooledObject))
            {
                // If the prefab doesn't have a PooledObject component, add one to handle pooling.
                pooledObject = instance.gameObject.AddComponent<PooledObject>();
            }

            if (pools.TryGetValue(prefab, out var pool))
            {
                pooledObject.SetPool(pool, instance);
            }
            else
            {
                Debug.LogError($"PoolManager: Pool for prefab {prefab.name} not found during creation.");
            }

            return instance;
        }
    }
}
