using System;

namespace UtilSNR.Common
{
    /// <summary>
    /// Generic thread-safe lazy-initialized pure C# singleton.
    /// T must have a public or private parameterless constructor.
    /// </summary>
    public abstract class TSingleton<T> where T : class, new()
    {
        private static readonly Lazy<T> lazyInstance = new Lazy<T>(() => new T());

        /// <summary>
        /// Gets the singleton instance, creating it on first access.
        /// </summary>
        public static T Instance => lazyInstance.Value;
    }
}