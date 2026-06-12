using UnityEngine;

namespace UtilSNR.Common
{
    /// <summary>
    /// Singleton MonoBehaviour scoped to a single scene.
    /// Instance is cleared when the GameObject is destroyed (e.g. scene unload),
    /// and will be re-resolved if accessed again in a new scene.
    /// </summary>
    public class TSceneSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        private static bool isApplicationQuitting = false;

        /// <summary>
        /// Gets the existing instance without creating a new one (may be null).
        /// </summary>
        protected static T ExistingInstance => instance;

        /// <summary>
        /// Gets the singleton instance, finding or creating one if necessary.
        /// </summary>
        public static T Instance => GetInstance();

        private static T GetInstance()
        {
            if (isApplicationQuitting)
                return null;

            if (instance == null)
            {
                instance = FindAnyObjectByType<T>();
                if (instance == null)
                {
                    var obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                }
            }
            return instance;
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Called when the application quits, prevents re-creation during shutdown.
        /// </summary>
        protected virtual void OnApplicationQuit()
        {
            isApplicationQuitting = true;
        }
    }
}
