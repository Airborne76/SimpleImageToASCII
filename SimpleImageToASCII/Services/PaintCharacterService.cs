using SimpleImageToASCII.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleImageToASCII.Services
{
    public class PaintCharacterService
    {
        private const string SettingsKey = "PaintCharacter";
        public static string PaintCharacter { get; set; } = "@";
        public static async Task InitializeAsync()
        {
            PaintCharacter = await LoadPaintCharacterFromSettingsAsync();
        }

        private static async Task<string> LoadPaintCharacterFromSettingsAsync()
        {
            string paintCharacter = "@";
            string paintCharacterName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
            if (!string.IsNullOrEmpty(paintCharacterName))
            {
                paintCharacter = paintCharacterName;
            }
            return paintCharacter;
        }
        public static async Task SetThemeAsync(string paintCharacter)
        {
            PaintCharacter = paintCharacter;
            await SaveFontSizeInSettingsAsync(paintCharacter);
        }

        private static async Task SaveFontSizeInSettingsAsync(string paintCharacter)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, paintCharacter.ToString());
        }
    }
}
