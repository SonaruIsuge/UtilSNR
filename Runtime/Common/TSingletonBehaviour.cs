using UnityEngine;
using System.Collections.Generic;

namespace UtilSNR.Common
{
    public class TSingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        private static bool isApplicationQuitting = false;

        protected static T ExistingInstance => instance;

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
                DontDestroyOnLoad(this.gameObject);
            }
            else if (instance == this)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
                isApplicationQuitting = true;
            }
        }

        protected void OnApplicationQuit()
        {
            isApplicationQuitting = true;
            instance = null;
        }
    }

}