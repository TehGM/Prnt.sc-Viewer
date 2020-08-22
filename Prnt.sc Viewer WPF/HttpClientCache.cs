using System;
using System.Net.Http;

namespace TehGM.PrntScViewer.WPF
{
    public class HttpClientCache : IDisposable
    {
        private readonly TimeSpan _cacheLifetime;
        private readonly string _userAgent;
        private readonly object _lock = new object();

        private DateTime _cachedTime;
        private HttpClient _client;

        private bool IsExpired => _client == null || DateTime.UtcNow > _cachedTime + _cacheLifetime;

        public HttpClientCache(TimeSpan cacheLifetime, string userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
                throw new ArgumentException("User agent is required", nameof(userAgent));

            this._cacheLifetime = cacheLifetime;
            this._userAgent = userAgent;
        }

        public HttpClient GetClient()
        {
            lock (_lock)
            {
                if (this.IsExpired)
                {
                    try { this._client?.Dispose(); } catch { }
                    this._client = new HttpClient();
                    this._client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
                    this._cachedTime = DateTime.UtcNow;
                }
                return this._client;
            }
        }

        public void Dispose()
        {
            this._client?.Dispose();
        }
    }
}
