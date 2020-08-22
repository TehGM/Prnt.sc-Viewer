using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TehGM.PrntScViewer;

namespace TehGM.PrntScViewer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScreenshotID CurrentScreenshotID
        {
            get => new ScreenshotID(ScreenshotIdBox.Text.Trim());
            set => ScreenshotIdBox.Text = value;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void GoToIdButton_Click(object sender, RoutedEventArgs e)
            => await DisplayImage(CurrentScreenshotID);

        private async void GoPreviousIdButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentScreenshotID--;
            await DisplayImage(CurrentScreenshotID);
        }

        private async void GoNextIdButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentScreenshotID++;
            await DisplayImage(CurrentScreenshotID);
        }

        private async Task DisplayImage(ScreenshotID id)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:72.0) Gecko/20100101 Firefox/72.0");
                byte[] imageBytes = await client.DownloadScreenshotBytesAsync(new ScreenshotID(id));
                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.UriSource = null;
                    image.StreamSource = stream;
                    image.EndInit();
                    image.Freeze();
                    ImageBox.Source = image;
                }
            }
        }
    }
}
