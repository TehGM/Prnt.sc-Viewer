using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TehGM.PrntScViewer
{
    public static class ScreenshotDownloadingExtensions
    {
        public static async Task<byte[]> DownloadScreenshotBytesAsync(this HttpClient client, ScreenshotID screenshotID, CancellationToken cancellationToken = default)
        {
            Uri screenshotUri = await GetImageUriAsync(client, screenshotID, cancellationToken);

            using (HttpResponseMessage screenshotResponse = await client.GetAsync(screenshotUri, cancellationToken))
                return await screenshotResponse.Content.ReadAsByteArrayAsync();
        }

        public static async Task<Stream> DownloadScreenshotStreamAsync(this HttpClient client, ScreenshotID screenshotID, CancellationToken cancellationToken = default)
        {
            Uri screenshotUri = await GetImageUriAsync(client, screenshotID, cancellationToken);

            using (HttpResponseMessage screenshotResponse = await client.GetAsync(screenshotUri, cancellationToken))
                return await screenshotResponse.Content.ReadAsStreamAsync();
        }

        private static async Task<Uri> GetImageUriAsync(HttpClient client, ScreenshotID screenshotID, CancellationToken cancellationToken = default)
        {
            Uri metadataUri = new Uri($"https://prnt.sc/{screenshotID}");

            using (HttpResponseMessage metadataResponse = await client.GetAsync(metadataUri, cancellationToken))
            {
                metadataResponse.EnsureSuccessStatusCode();

                HtmlDocument html = new HtmlDocument();
                html.Load(await metadataResponse.Content.ReadAsStreamAsync());
                // first attempt to get image from og:image meta tag
                string metaImageUrl = html.DocumentNode.SelectSingleNode("//meta[@property='og:image']").GetAttributeValue("content", null);
                // if null, try twitter:image:src
                if (string.IsNullOrWhiteSpace(metaImageUrl))
                    metaImageUrl = html.DocumentNode.SelectSingleNode("//meta[@name='twitter:image:src']").GetAttributeValue("content", null);
                // if failed, throw
                if (string.IsNullOrWhiteSpace(metaImageUrl))
                    throw new KeyNotFoundException("No raw image URL found in Prnt.sc's response");

                if (metaImageUrl.StartsWith("//"))
                    metaImageUrl = metaImageUrl.Insert(0, "https:");
                return new Uri(metaImageUrl);
            }
        }
    }
}
