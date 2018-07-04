using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleImageToASCII.Helpers;

using Windows.Storage;

namespace SimpleImageToASCII.Services
{
    public class PixelSelectorService
    {
        private const string SettingsKey = "PixelDensity";
        public static int PixelDensity { get; set; } = 1;
        public static async Task InitializeAsync()
        {
            PixelDensity = await LoadPixelDensityFromSettingsAsync();
        }

        private static async Task<int> LoadPixelDensityFromSettingsAsync()
        {
            int pixelDensity = 1;
            string pixelDensityName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
            if (!string.IsNullOrEmpty(pixelDensityName))
            {
                int.TryParse(pixelDensityName, out pixelDensity);
            }
            return pixelDensity;
        }
        public static async Task SetThemeAsync(int pixelDensity)
        {
            PixelDensity = pixelDensity;
            await SaveFontSizeInSettingsAsync(pixelDensity);
        }

        private static async Task SaveFontSizeInSettingsAsync(int pixelDensity)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, pixelDensity.ToString());
        }
    }
}
