using SimpleImageToASCII.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleImageToASCII.Services
{
    public class BackgroundImageService
    {
        private const string SettingsKey = "UseBackgroundImage";
        public static bool UseBackgroundImage { get; set; } = false;
        public static async Task InitializeAsync()
        {
            UseBackgroundImage = await LoadBackgroundImageFromSettingsAsync();
        }

        public static async Task SetThemeAsync(bool useBackgroundImage)
        {
            UseBackgroundImage = useBackgroundImage;
            await SaveBackgroundImageInSettingsAsync(useBackgroundImage);
        }

        private static async Task<bool> LoadBackgroundImageFromSettingsAsync()
        {
            bool useBackgroundImage = false;
            string useBackgroundImageName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
            if (!string.IsNullOrEmpty(useBackgroundImageName))
            {
                Boolean.TryParse(useBackgroundImageName, out useBackgroundImage);
            }
            return useBackgroundImage;
        }

        private static async Task SaveBackgroundImageInSettingsAsync(bool useBackgroundImage)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, useBackgroundImage.ToString());
        }

    }
}
