using SimpleImageToASCII.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleImageToASCII.Services
{
    public class FontSizeService
    {
        private const string SettingsKey = "FontSize";
        public static int FontSize { get; set; } = 1;
        public static async Task InitializeAsync()
        {
            FontSize = await LoadFontSizeFromSettingsAsync();
        }

        private static async Task<int> LoadFontSizeFromSettingsAsync()
        {
            int fontSize = 1;
            string fontSizeName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
            if (!string.IsNullOrEmpty(fontSizeName))
            {
                int.TryParse(fontSizeName, out fontSize);
            }
            return fontSize;
        }
        public static async Task SetThemeAsync(int fontSize)
        {
            FontSize = fontSize;
            await SaveFontSizwInSettingsAsync(fontSize);
        }

        private static async Task SaveFontSizwInSettingsAsync(int fontSize)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, fontSize.ToString());
        }


    }
}
