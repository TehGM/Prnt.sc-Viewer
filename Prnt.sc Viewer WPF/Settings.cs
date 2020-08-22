using System;
using Newtonsoft.Json;

namespace TehGM.PrntScViewer.WPF
{
    public class Settings
    {
        [JsonProperty("UserAgent")]
        public string UserAgent { get; set; } = "TehGM's Prnt.sc Viewer";

        [JsonProperty("HttpClientCacheLifetime")]
        public TimeSpan HttpClientCacheLifetime { get; set; } = TimeSpan.FromMinutes(10);

        [JsonProperty("ResetOnLoad")]
        public bool ResetOnLoad { get; set; } = true;
    }
}
