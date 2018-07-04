using System;

using SimpleImageToASCII.ViewModels;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SimpleImageToASCII.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ViewModel.Init(canvasControl);
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
            base.OnNavigatedFrom(e);
        }

        private void Dispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs args)
        {
            if (args.EventType.ToString().Contains("Down")) 
            {
                var ctrl = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
                if (ctrl.HasFlag(CoreVirtualKeyStates.Down))
                {
                    switch (args.VirtualKey)
                    {
                        case VirtualKey.V:
                            ViewModel.PasteImage();
                            break;
                    }
                }
            }
        }

        private void Page_DragOver(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            DragIcon.Visibility = Visibility.Visible;
        }

        private void Page_DragLeave(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            DragIcon.Visibility = Visibility.Collapsed;
        }

        private void Page_Drop(object sender, Windows.UI.Xaml.DragEventArgs e)
        {
            DragIcon.Visibility = Visibility.Collapsed;
        }
    }
}
