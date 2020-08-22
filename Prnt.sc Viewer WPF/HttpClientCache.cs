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
                    this._client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:72.0) Gecko/20100101 Firefox/72.0");
                    this._cachedTime = DateTime.UtcNow;
                }
                return this._client;
            }
        }
    }
}
