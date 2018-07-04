using SimpleImageToASCII.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleImageToASCII.Services
{
    public class CharacterService
    {
        private const string SettingsKey = "Characters";
        public static string Characters { get; set; } = " .,:;i1tfLCG08@";
        public static async Task InitializeAsync()
        {
            Characters = await LoadCharactersFromSettingsAsync();
        }

        private static async Task<string> LoadCharactersFromSettingsAsync()
        {
            string characters = " .,:;i1tfLCG08@";
            string charactersName = await ApplicationData.Current.LocalSettings.ReadAsync<string>(SettingsKey);
            if (!string.IsNullOrEmpty(charactersName))
            {
                //int.TryParse(charactersName, out characters);
                characters = charactersName;
            }
            return characters;
        }
        public static async Task SetThemeAsync(string characters)
        {
            Characters = characters;
            await SaveFontSizeInSettingsAsync(characters);
        }

        private static async Task SaveFontSizeInSettingsAsync(string characters)
        {
            await ApplicationData.Current.LocalSettings.SaveAsync(SettingsKey, characters.ToString());
        }


    }
}
