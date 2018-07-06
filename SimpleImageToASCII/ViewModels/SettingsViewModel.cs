using System;
using System.Windows.Input;
using SimpleImageToASCII.Helpers;
using SimpleImageToASCII.Models;
using SimpleImageToASCII.Services;
using SimpleImageToASCII.Views;
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Xaml;

namespace SimpleImageToASCII.ViewModels
{
    // TODO WTS: Add other settings as necessary. For help see https://github.com/Microsoft/WindowsTemplateStudio/blob/master/docs/pages/settings.md
    public class SettingsViewModel : Observable
    {
        private ElementTheme _elementTheme = ThemeSelectorService.Theme;

        public ElementTheme ElementTheme
        {
            get { return _elementTheme; }

            set { Set(ref _elementTheme, value); }
        }

        private string _versionDescription;

        public string VersionDescription
        {
            get { return _versionDescription; }

            set { Set(ref _versionDescription, value); }
        }

        private ICommand _switchThemeCommand;

        public ICommand SwitchThemeCommand
        {
            get
            {
                if (_switchThemeCommand == null)
                {
                    _switchThemeCommand = new RelayCommand<ElementTheme>(
                        async (param) =>
                        {
                            ElementTheme = param;
                            await ThemeSelectorService.SetThemeAsync(param);
                        });
                }

                return _switchThemeCommand;
            }
        }

        private bool _useBackGroundImage = BackgroundImageService.UseBackgroundImage;

        public bool UseBackGroundImage
        {
            get { return _useBackGroundImage; }
            set
            {
                Set(ref _useBackGroundImage, value);
                BackgroundImageService.SetThemeAsync(value);
            }
        }

        private bool _usePictureCompression = PictureCompression.UsePictureCompression;

        public bool UsePictureCompression
        {
            get { return _usePictureCompression; }
            set
            {
                Set(ref _usePictureCompression, value);
                PictureCompression.SetThemeAsync(value);
            }
        }

        private string _characters = CharacterService.Characters;

        public string Characters
        {
            get { return _characters; }
            set
            {
                if (value.Length < 15)
                {
                    value = value.PadLeft(15, ' ');
                }
                Set(ref _characters, value);
                CharacterService.SetThemeAsync(value);
            }
        }

        public ICommand SetToDefault
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Characters = DefaultCharacters.defaultCharacters;
                });
            }
        }

        private ICommand _whatsnewCommand;

        public ICommand WhatsNewCommand
        {
            get
            {
                if (_whatsnewCommand == null)
                {
                    _whatsnewCommand = new RelayCommand(async () =>
                    {
                        var dialog = new WhatsNewDialog();
                        await dialog.ShowAsync();
                    });
                }
                return _whatsnewCommand;
            }
        }

        private ICommand _sendFeedback;

        public ICommand SendFeedback
        {
            get
            {
                if (_sendFeedback==null)
                {
                    _sendFeedback = new RelayCommand(async() =>
                    {
                        string url = "ms-windows-store://review/?ProductId=9PD60ZWCB16K";
                        await Launcher.LaunchUriAsync(new Uri(url));
                    });
                }
                return _sendFeedback;
            }
        }
        public ICommand ShareApplication
        {
            get
            {
                return new RelayCommand(() =>
                {
                    dataTransferManager.DataRequested += OnDataRequested;
                    DataTransferManager.ShowShareUI();
                });
            }
        }

        DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            ShareSourceData shareSourceData = new ShareSourceData("AppDisplayName".GetLocalized());
            shareSourceData.SetWebLink(new Uri("https://www.microsoft.com/store/productId/9PD60ZWCB16K"));
            e.Request.SetData(shareSourceData);
        }

        public SettingsViewModel()
        {
        }

        public void Initialize()
        {
            VersionDescription = GetVersionDescription();
        }

        private string GetVersionDescription()
        {
            var appName = "AppDisplayName".GetLocalized();
            var package = Package.Current;
            var packageId = package.Id;
            var version = packageId.Version;

            return $"{appName} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }
}
