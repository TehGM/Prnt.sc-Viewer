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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartLoading();
            HttpClient client = App.HttpClientCache.GetClient();
            int statsScreenshotID = await client.DownloadScreenshotsUploadedCountAsync();
            this.CurrentScreenshotID = (ScreenshotID)statsScreenshotID;
            await DisplayImage(CurrentScreenshotID);
        }

        private async void GoToIdButton_Click(object sender, RoutedEventArgs e)
            => await DisplayImage(CurrentScreenshotID);

        private async void GoPreviousIdButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentScreenshotID--;
            await DisplayImage(CurrentScreenshotID);
        }

        private async void GoNextIdButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentScreenshotID++;
            await DisplayImage(CurrentScreenshotID);
        }

        private async Task DisplayImage(ScreenshotID id)
        {
            StartLoading();
            HttpClient client = App.HttpClientCache.GetClient();
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
            StopLoading();
        }

        private void StartLoading()
        {
            this.LoadingSpinner.Visibility = Visibility.Visible;
            this.ScreenshotIdBox.IsReadOnly = true;
        }
        private void StopLoading()
        {
            this.LoadingSpinner.Visibility = Visibility.Collapsed;
            this.ScreenshotIdBox.IsReadOnly = false;
        }
    }
}
