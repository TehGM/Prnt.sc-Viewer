using System;
using System.IO;
using System.Linq;

namespace TehGM.PrntScViewer
{
    public class Screenshot
    {
        public ScreenshotID ID { get; }
        public byte[] Data { get; }
        public string DirectURL { get; }

        public string FileName => Path.GetFileName(this.DirectURL);

        public Screenshot(ScreenshotID id, byte[] data, string directUrl)
        {
            if (data?.Any() != true)
                throw new ArgumentException("Screenshot byte data is required", nameof(data));
            if (string.IsNullOrWhiteSpace(directUrl))
                throw new ArgumentException("Screenshot URL is required", nameof(directUrl));

            this.ID = id;
            this.Data = data;
            this.DirectURL = directUrl;
        }
    }
}
