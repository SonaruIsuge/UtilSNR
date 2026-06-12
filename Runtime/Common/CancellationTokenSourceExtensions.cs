using System.Threading;
using UnityEngine;

namespace UtilSNR.Common
{
    /// <summary>
    /// Extension methods for CancellationTokenSource to simplify common operations like canceling and refreshing tokens.
    /// </summary>
    public static class CancellationTokenSourceExtensions
    {
        /// <summary>
        /// Cancels the token and disposes the CancellationTokenSource. 
        /// This is a common pattern to ensure that resources are cleaned up properly.
        /// </summary>
        /// <param name="cts"></param>
        public static void CancelAndDispose(this CancellationTokenSource cts)
        {
            if (cts == null) 
                return;
            
            cts.Cancel();
            cts.Dispose();
        }

        /// <summary>
        /// Cancels the existing token and creates a new CancellationTokenSource.
        /// </summary>
        /// <param name="cts"></param>
        /// <returns></returns>
        public static CancellationTokenSource Refresh(this CancellationTokenSource cts)
        {
            cts?.CancelAndDispose();
            return new CancellationTokenSource();
        }
    }
}
