using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SimpleImageToASCII.Views
{
    public sealed partial class WhatsNewDialog : ContentDialog
    {
        public WhatsNewDialog()
        {
            // TODO WTS: Update the contents of this dialog every time you release a new version of the app
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            InitializeComponent();
        }

        //private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        //{
        //    string url = "ms-windows-store://review/?ProductId=9PD60ZWCB16K";
        //    await Launcher.LaunchUriAsync(new Uri(url));
        //}

        private async void HyperlinkButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            string url = "ms-windows-store://review/?ProductId=9PD60ZWCB16K";
            await Launcher.LaunchUriAsync(new Uri(url));
        }
    }
}
