using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TehGM.PrntScViewer.WPF
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly Brush _normalBoxBorderBrush;

        public SettingsWindow()
        {
            InitializeComponent();
            this._normalBoxBorderBrush = this.UserAgentBox.BorderBrush;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.UserAgentBox.Text = App.Settings.UserAgent;
            this.ResetOnLoadBox.IsChecked = App.Settings.ResetOnLoad;
        }

        private void UserAgentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateUserAgentInput();
        }

        private bool ValidateUserAgentInput()
        {
            bool isValid = !string.IsNullOrWhiteSpace(this.UserAgentBox.Text);
            this.UserAgentBox.BorderBrush = isValid ? _normalBoxBorderBrush : App.ErrorBrush;
            this.SaveButton.Foreground = isValid ? App.DefaultForegroundBrush : App.ErrorBrush;
            return isValid;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateUserAgentInput())
            {
                MessageBox.Show(this, "User agent is required!", this.Title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            try
            {
                Settings settings = new Settings();
                settings.ResetOnLoad = this.ResetOnLoadBox.IsChecked == true;
                settings.UserAgent = this.UserAgentBox.Text.Trim();
                await App.UpdateSettingsAsync(settings);
                this.DialogResult = true;
                this.Close();
            }
            catch
            {
                MessageBox.Show(this, "Error saving settings", this.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
