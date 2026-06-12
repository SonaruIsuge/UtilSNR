using UnityEngine;
using UnityEngine.Pool;

namespace UtilSNR.Pool
{
    /// <summary>
    /// Component with this interface can be pooled by PoolManager.
    /// It provides methods to set the pool and return to the pool,
    /// as well as hooks for when the object is spawned and despawned.
    /// </summary>
    public interface IPoolable
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //          Non-Inspector Fields
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        IObjectPool<Component> pool { get; set; }
        Component instance { get; set; }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //          Methods
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public void SetPool(IObjectPool<Component> pool, Component instance)
        {
            this.pool = pool;
            this.instance = instance;
        }

        public void ReturnToPool()
        {
            if (pool != null)
            {
                pool.Release(instance);
            }
            else
            {
                if (instance != null)
                    Object.Destroy(instance.gameObject);
            }
        }

        public void OnSpawn();
        public void OnDespawn();
    }
}

