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

        private readonly Brush _errorBrush;
        private readonly Brush _defaultForegroundBrush;
        private readonly Brush _normalScreenshotIdBoxBorderBrush;
        public MainWindow()
        {
            InitializeComponent();
            this._errorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            this._normalScreenshotIdBoxBorderBrush = ScreenshotIdBox.BorderBrush;
            this._defaultForegroundBrush = (Brush)FindResource("DefaultForegroundBrush");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.ValidateScreenshotIdInput();
            await ResetScreenshotIDAsync();
        }

        private async void GoToIdButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ScreenshotID.Validate(ScreenshotIdBox.Text))
                this.WriteStatusError("Incorrect Screenshot ID.", true);
            else
                await DisplayImageAsync(new ScreenshotID(ScreenshotIdBox.Text));
        }

        private async Task DisplayImageAsync(ScreenshotID id)
        {
            this.StartLoading();
            try
            {
                HttpClient client = App.HttpClientCache.GetClient();
                this.WriteStatusNormal($"Downloading image {id}...");
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
                this.WriteStatusNormal("Done.");
            }
            catch
            {
                this.WriteStatusError($"Failed loading image {id}.");
            }
            finally
            {
                this.StopLoading();
            }
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
            this.ValidateScreenshotIdInput();
        }

        private void ImageBox_CopyLink_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScreenshot == null)
            {
                this.WriteStatusError("Please load a screenshot first!", true);
                return;
            }
            Clipboard.SetText(CurrentScreenshotURL);
            this.WriteStatusNormal("Image link copied!");
        }

        private void ImageBox_OpenBrowser_Click(object sender, RoutedEventArgs e)
        {
            if (_currentScreenshot == null)
            {
                this.WriteStatusError("Please load a screenshot first!", true);
                return;
            }
            using Process prc = new Process();
            prc.StartInfo.FileName = CurrentScreenshotURL;
            prc.StartInfo.UseShellExecute = true;
            prc.Start();
        }

        private void WriteStatusNormal(string text, bool popup = false, MessageBoxImage image = MessageBoxImage.Information)
            => WriteStatus(text, _defaultForegroundBrush, popup, image);
        private void WriteStatusError(string text, bool popup = false, MessageBoxImage image = MessageBoxImage.Error)
            => WriteStatus(text, _errorBrush, popup, image);

        private void WriteStatus(string text, Color color, bool popup = false, MessageBoxImage image = MessageBoxImage.Information)
            => this.WriteStatus(text, new SolidColorBrush(color), popup, image);

        private void WriteStatus(string text, Brush color, bool popup = false, MessageBoxImage image = MessageBoxImage.Information)
        {
            this.StatusTextBox.Text = text;
            this.StatusTextBox.Foreground = color;
            if (popup)
                MessageBox.Show(this, text, this.Title, MessageBoxButton.OK, image);
        }

        private void ScreenshotIdBox_TextChanged(object sender, TextChangedEventArgs e)
            => this.ValidateScreenshotIdInput();

        private bool ValidateScreenshotIdInput()
        {
            bool isValid = ScreenshotID.Validate(this.ScreenshotIdBox.Text);
            this.ScreenshotIdBox.BorderBrush = isValid ? _normalScreenshotIdBoxBorderBrush : _errorBrush;
            this.GoToIdButton.IsEnabled = isValid;
            this.GoToIdButton.Foreground = isValid ? _defaultForegroundBrush : _errorBrush;
            this.ScreenshotInvalidWarning.Visibility = isValid ? Visibility.Collapsed : Visibility.Visible;
            return isValid;
        }

        private async Task ResetScreenshotIDAsync()
        {
            this.StartLoading();
            try
            {
                HttpClient client = App.HttpClientCache.GetClient();
                this.WriteStatusNormal("Requesting uploaded image count...");
                int statsScreenshotID = await client.DownloadScreenshotsUploadedCountAsync();
                await DisplayImageAsync((ScreenshotID)statsScreenshotID);
            }
            catch
            {
                this.WriteStatusError("Failed loading initial image.");
            }
            finally
            {
                this.StopLoading();
            }
        }

        private async void Command_Reset(object sender, ExecutedRoutedEventArgs e)
        {
            await ResetScreenshotIDAsync();
        }

        private async void Command_Save(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentScreenshot == null)
            {
                this.WriteStatusError("Please load a screenshot first!", true);
                return;
            }
            this.StartLoading();
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = this._currentScreenshot.FileName;
                string ext = Path.GetExtension(this._currentScreenshot.FileName);
                saveFileDialog.Filter = $"Image File|*{ext}";
                saveFileDialog.DefaultExt = ext;
                if (saveFileDialog.ShowDialog(this) == true)
                {
                    this.WriteStatusNormal($"Saving {saveFileDialog.FileName}...");
                    await File.WriteAllBytesAsync(saveFileDialog.FileName, this._currentScreenshot.Data);
                }
                this.WriteStatusNormal("Done.");
            }
            catch
            {
                this.WriteStatusError("Error saving image.", true);
            }
            finally
            {
                this.StopLoading();
            }
        }

        private void Command_Copy(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentScreenshot == null)
            {
                this.WriteStatusError("Please load a screenshot first!", true);
                return;
            }
            Clipboard.SetImage((BitmapImage)this.ImageBox.Source);
            this.WriteStatusNormal("Image copied!");
        }

        private async void Command_Next(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentScreenshot == null)
            {
                this.WriteStatusError("Please load a screenshot first!", true);
                return;
            }
            await DisplayImageAsync(_currentScreenshot.ID.Increment());
        }

        private async void Command_Previous(object sender, ExecutedRoutedEventArgs e)
        {
            if (_currentScreenshot == null)
            {
                this.WriteStatusError("Please load a screenshot first!", true);
                return;
            }
            await DisplayImageAsync(_currentScreenshot.ID.Decrement());
        }
    }
}
