using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace TehGM.PrntScViewer
{
    public static class PrntScStatsDownloadingExtensions
    {
        public static async Task<int> DownloadScreenshotsUploadedCountAsync(this HttpClient client, CancellationToken cancellationToken = default)
        {
            Uri statsUri = new Uri($"https://prnt.sc/");

            using (HttpResponseMessage statsResponse = await client.GetAsync(statsUri, cancellationToken))
            {
                statsResponse.EnsureSuccessStatusCode();

                HtmlDocument html = new HtmlDocument();
                html.Load(await statsResponse.Content.ReadAsStreamAsync());
                string screenshotsCountRaw = html.DocumentNode.SelectSingleNode("//div[@class='loaded-info__numbs']").GetDirectInnerText();
                // get rid of spaces
                screenshotsCountRaw = screenshotsCountRaw.Replace(" ", string.Empty);
                return int.Parse(screenshotsCountRaw);
            }
        }
    }
}
