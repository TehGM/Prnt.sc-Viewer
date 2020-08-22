using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TehGM.PrntScViewer.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static HttpClientCache HttpClientCache { get; private set; }
        public static Settings Settings { get; private set; }

        public static Brush ErrorBrush { get; private set; }
        public static Brush DefaultForegroundBrush { get; private set; }


        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            ErrorBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            DefaultForegroundBrush = (Brush)FindResource("DefaultForegroundBrush");

            await LoadSettingsAsync();
        }

        public static async Task UpdateSettingsAsync(Settings settings)
        {
            Settings = settings;
            InitializeHttpClientCache();

            string dir = GetSettingsFileDirectory();
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string path = GetSettingsFilePath();
            using FileStream file = File.Create(path);
            using StreamWriter streamWriter = new StreamWriter(file);
            using JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter);
            await JObject.FromObject(Settings).WriteToAsync(jsonWriter);
        }

        public static async Task<Settings> LoadSettingsAsync()
        {
            Settings = await LoadSettingsInternalAsync();
            InitializeHttpClientCache();
            return Settings;
        }

        private static void InitializeHttpClientCache()
        {
            try { HttpClientCache?.Dispose(); } catch { }
            HttpClientCache = new HttpClientCache(Settings.HttpClientCacheLifetime, Settings.UserAgent);
        }

        private static async Task<Settings> LoadSettingsInternalAsync()
        {
            string path = GetSettingsFilePath();
            if (!File.Exists(path))
            {
                return new Settings();
            }
            using StreamReader file = File.OpenText(path);
            using JsonTextReader reader = new JsonTextReader(file);
            JObject json = await JObject.LoadAsync(reader);
            return json.ToObject<Settings>();
        }

        public static string GetSettingsFilePath()
            => Path.Combine(GetSettingsFileDirectory(), "settings.json");

        public static string GetSettingsFileDirectory()
            => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TehGM", "PrntscViewer");
    }
}
