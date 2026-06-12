using UnityEngine;
using UnityEngine.Pool;

namespace UtilSNR.Pool
{
    /// <summary>
    /// Default implementation of IPoolable. 
    /// </summary>
    public class PooledObject : MonoBehaviour, IPoolable
    {
        public IObjectPool<Component> pool { get; set; }
        public Component instance { get; set; }

        public void OnSpawn() { }
        public void OnDespawn() { }
    }
}
