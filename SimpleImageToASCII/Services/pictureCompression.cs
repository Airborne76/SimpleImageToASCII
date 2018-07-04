using System;
using SimpleImageToASCII.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleImageToASCII.Services
{
    public class PictureCompression
    {
        private const string SettingsKey = "pictureCompression";
        public static bool UsePictureCompression { get; set; } = false;
        public static async Task InitializeAsync()
        {
            UsePictureCompression = await LoadPictureCompressionFromSettingsAsync();
        }

        public static async Task SetThemeAsync(bool usePictureCompression)
        {
            UsePictureCompression = usePictureCompression;
            await SavePictureCompressionInSettingsAsync(usePictureCompression);
        }

        private static async Task<bool> LoadPictureCompressionFromSettingsAsync()
        {
            bool usePictureCompression = false;
            string usePictureCompressionName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
            if (!string.IsNullOrEmpty(usePictureCompressionName))
            {
                Boolean.TryParse(usePictureCompressionName, out usePictureCompression);
            }
            return usePictureCompression;
        }

        private static async Task SavePictureCompressionInSettingsAsync(bool usePictureCompression)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, usePictureCompression.ToString());
        }

    }
}
