using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace TehGM.PrntScViewer.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Screenshot _currentScreenshot;
        private string CurrentScreenshotURL => $"https://prnt.sc/{this._currentScreenshot.ID}";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.StartLoading();
            HttpClient client = App.HttpClientCache.GetClient();
            int statsScreenshotID = await client.DownloadScreenshotsUploadedCountAsync();
            await DisplayImageAsync((ScreenshotID)statsScreenshotID);
        }

        private async void GoToIdButton_Click(object sender, RoutedEventArgs e)
            => await DisplayImageAsync(new ScreenshotID(ScreenshotIdBox.Text));

        private async void GoPreviousIdButton_Click(object sender, RoutedEventArgs e)
            => await DisplayImageAsync(_currentScreenshot.ID.Decrement());

        private async void GoNextIdButton_Click(object sender, RoutedEventArgs e)
            => await DisplayImageAsync(_currentScreenshot.ID.Increment());

        private async Task DisplayImageAsync(ScreenshotID id)
        {
            this.StartLoading();
            HttpClient client = App.HttpClientCache.GetClient();
            this._currentScreenshot = await client.DownloadScreenshotAsync(id);
            using (MemoryStream stream = new MemoryStream(this._currentScreenshot.Data))
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
            this.ScreenshotIdBox.Text = this._currentScreenshot.ID.ToString();
            this.StopLoading();
        }

        private void StartLoading()
        {
            this.LoadingSpinner.Visibility = Visibility.Visible;
            this.ScreenshotIdBox.IsEnabled = false;
            this.GoNextIdButton.IsEnabled = false;
            this.GoPreviousIdButton.IsEnabled = false;
            this.GoToIdButton.IsEnabled = false;
        }
        private void StopLoading()
        {
            this.LoadingSpinner.Visibility = Visibility.Collapsed;
            this.ScreenshotIdBox.IsEnabled = true;
            this.GoNextIdButton.IsEnabled = true;
            this.GoPreviousIdButton.IsEnabled = true;
            this.GoToIdButton.IsEnabled = true;
        }

        private async void ImageBox_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            this.StartLoading();
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = this._currentScreenshot.FileName;
            string ext = Path.GetExtension(this._currentScreenshot.FileName);
            saveFileDialog.Filter = $"Image File|*{ext}";
            saveFileDialog.DefaultExt = ext;
            if (saveFileDialog.ShowDialog(this) == true)
                await File.WriteAllBytesAsync(saveFileDialog.FileName, this._currentScreenshot.Data);
            this.StopLoading();
        }

        private void ImageBox_CopyImage_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetImage((BitmapImage)this.ImageBox.Source);
        }

        private void ImageBox_CopyLink_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentScreenshotURL);
        }

        private void ImageBox_OpenBrowser_Click(object sender, RoutedEventArgs e)
        {
            using Process prc = new Process();
            prc.StartInfo.FileName = CurrentScreenshotURL;
            prc.StartInfo.UseShellExecute = true;
            prc.Start();
        }
    }
}
