using Microsoft.Extensions.Primitives;

namespace StealAllTheCats.Infastracture
{
    /// <summary>
    /// Provides a cancellation token that can be used as an expiration token for caching mechanisms.
    /// When the token is expired, any cache entries using it can be invalidated.
    /// </summary>
    public class CacheTokenProvider : ICacheTokenProvider
    {
        private CancellationTokenSource _cts = new();
        
        /// <summary>
        /// Gets a <see cref="CancellationChangeToken"/> 
        /// This token can be used as an expiration token for cache entries.
        /// </summary>
        public CancellationChangeToken GetToken() => new(_cts.Token);

        /// <summary>
        /// Expires the current cancellation token, signaling that cache entries using this token should be invalidated.
        /// It then creates and assigns a new cancellation token source for further use.
        /// </summary>
        public void ExpireToken()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }
        }
    }
}
