using SimpleImageToASCII.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleImageToASCII.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class EmptyDialog : ContentDialog
    {
        public EmptyDialog()
        {
            RequestedTheme = (Window.Current.Content as FrameworkElement).RequestedTheme;
            this.InitializeComponent();
        }

        public int PixelWidth { get; set; } = 50;
        public int PixelHeight { get; set; } = 50;

        public bool IsCancel { get; set; } = true;
        private ICommand Confirm
        {
            get
            {
                return new RelayCommand(() =>
                {
                    IsCancel = false;
                    //var length = PixelWidth + PixelHeight;
                });
            }
        }
    }
}
