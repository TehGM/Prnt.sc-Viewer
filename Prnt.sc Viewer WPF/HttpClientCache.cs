using System;
using System.Net.Http;

namespace TehGM.PrntScViewer.WPF
{
    public class HttpClientCache
    {
        private readonly TimeSpan _cacheLifetime;
        private readonly object _lock = new object();

        private DateTime _cachedTime;
        private HttpClient _client;

        private bool IsExpired => _client == null || DateTime.UtcNow > _cachedTime + _cacheLifetime;

        public HttpClientCache(TimeSpan cacheLifetime)
        {
            this._cacheLifetime = cacheLifetime;
        }

        public HttpClient GetClient()
        {
            lock (_lock)
            {
                if (this.IsExpired)
                {
                    try { this._client?.Dispose(); } catch { }
                    this._client = new HttpClient();
                    this._client.DefaultRequestHeaders.Add("User-Agent", "TehGM's Prnt.sc Viewer");
                    this._cachedTime = DateTime.UtcNow;
                }
                return this._client;
            }
        }
    }
}
