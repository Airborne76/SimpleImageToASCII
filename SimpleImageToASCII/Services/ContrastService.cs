using SimpleImageToASCII.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleImageToASCII.Services
{
     public class ContrastService
    {
        private const string SettingsKey = "Contrast";
        public static int Contrast { get; set; } = 128;
        public static async Task InitializeAsync()
        {
            Contrast = await LoadContrastDensityFromSettingsAsync();
        }

        private static async Task<int> LoadContrastDensityFromSettingsAsync()
        {
            int contrast = 1;
            string contrastName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
            if (!string.IsNullOrEmpty(contrastName))
            {
                int.TryParse(contrastName, out contrast);
            }
            return contrast;
        }
        public static async Task SetThemeAsync(int contrast)
        {
            Contrast = contrast;
            await SaveFontSizeInSettingsAsync(contrast);
        }

        private static async Task SaveFontSizeInSettingsAsync(int contrast)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, contrast.ToString());
        }

    }
}
