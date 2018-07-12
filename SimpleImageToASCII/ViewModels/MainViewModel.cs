using System;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Windows.Foundation;
using SimpleImageToASCII.Helpers;
using Microsoft.Graphics.Canvas;
using SimpleImageToASCII.Models;
using Windows.UI;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using Windows.UI.Xaml;
using System.Windows.Input;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Input;
using System.Collections.Generic;
using SimpleImageToASCII.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media;
using SimpleImageToASCII.Views;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.Graphics.Display;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System;
using System.Diagnostics;

namespace SimpleImageToASCII.ViewModels
{
    public class MainViewModel : Observable
    {
        public MainViewModel()
        {

        }
        private int contrast = ContrastService.Contrast;
        public int Contrast
        {
            get { return contrast; }
            set
            {
                Set(ref contrast, value);
                ContrastService.SetThemeAsync(value);
            }
        }

        public CanvasControl canvasControl;
        public void Init(CanvasControl canvasControl)
        {
            this.canvasControl = canvasControl;
        }

        private Visibility visibility = Visibility.Collapsed;
        public Visibility CanvasVisibility
        {
            get { return visibility; }
            set { Set(ref visibility, value); }
        }

        private string _ASCIIText = "";
        public string ASCIIText
        {
            get { return _ASCIIText; }
            set { Set(ref _ASCIIText, value); }
        }

        private int _PixelDensity = PixelSelectorService.PixelDensity;
        public int PixelDensity
        {
            get { return _PixelDensity; }
            set
            {
                Set(ref _PixelDensity, value);
                PixelSelectorService.SetThemeAsync(value);
            }
        }

        private string _PaintingCharacter = PaintCharacterService.PaintCharacter;

        public string PaintingCharacter
        {
            get { return _PaintingCharacter; }
            set
            {
                if (value=="")
                {
                    value = " ";
                }
                Set(ref _PaintingCharacter, value);
                PaintCharacterService.SetThemeAsync(value);
            }
        }


        private int _FontSize = FontSizeService.FontSize;
        public int FontSize
        {
            get { return _FontSize; }
            set
            {
                Set(ref _FontSize, value);
                FontSizeService.SetThemeAsync(value);
            }
        }

        private BitmapImage bitmapImage = new BitmapImage();
        public BitmapImage BitmapImage
        {
            get { return bitmapImage; }
            set { Set(ref bitmapImage, value); }
        }

        CanvasBitmap canvasBitmap;
        public TypedEventHandler<CanvasControl, CanvasDrawEventArgs> Draw
        {
            get
            {
                return (s, e) =>
                {
                    using (var session = e.DrawingSession)
                    {
                        if (canvasBitmap != null)
                        {
                            session.DrawImage(canvasBitmap);
                        }
                    }
                };
            }
        }

        public RightTappedEventHandler ShowFlyout
        {
            get
            {
                return (s, e) =>
                {
                    Border Element = s as Border;
                    if (ASCIIText != string.Empty)
                    {
                        MenuFlyout flyout = new MenuFlyout();
                        MenuFlyoutItem convertText = new MenuFlyoutItem { Text = "MainPage_SaveAsImage".GetLocalized() };
                        convertText.Click += async (sender, eventArgs) =>
                        {
                            StorageFile file = await KnownFolders.PicturesLibrary.CreateFileAsync("ASCII_Image.png", CreationCollisionOption.GenerateUniqueName);
                            Element.Background = new SolidColorBrush(Colors.White);
                            var bitmap = new RenderTargetBitmap();
                            await bitmap.RenderAsync(Element);
                            var buffer = await bitmap.GetPixelsAsync();
                            using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                var encod = await BitmapEncoder.CreateAsync(
                                    BitmapEncoder.PngEncoderId, stream);
                                encod.SetPixelData(BitmapPixelFormat.Bgra8,
                                    BitmapAlphaMode.Ignore,
                                    (uint)bitmap.PixelWidth,
                                    (uint)bitmap.PixelHeight,
                                    DisplayInformation.GetForCurrentView().LogicalDpi,
                                    DisplayInformation.GetForCurrentView().LogicalDpi,
                                    buffer.ToArray()
                                   );
                                await encod.FlushAsync();
                            }
                            Element.Background = new SolidColorBrush(Colors.Transparent);
                            UIDispatcher.RunAsync(() =>
                            {
                                new ToastNotificationsService().ShowToastNotificationSample(file.Path);
                            });
                        };
                        flyout.Items.Add(convertText);
                        MenuFlyoutItem copyText = new MenuFlyoutItem { Text = "MainPage_CopyText".GetLocalized() };
                        copyText.Click += (sender, eventArgs) =>
                        {
                            var dataPakage = new DataPackage();
                            dataPakage.SetText(ASCIIText);
                            try
                            {
                                Clipboard.SetContent(dataPakage);
                            }
                            catch (Exception)
                            {

                            }
                        };
                        flyout.Items.Add(copyText);
                        flyout.ShowAt(Element);
                        //FlyoutBase.ShowAttachedFlyout((s as TextBlock));
                    }
                };
            }
        }

        public ICommand LoadImage
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    FileOpenPicker filePicker = new FileOpenPicker();
                    filePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                    filePicker.FileTypeFilter.Add(".jpg");
                    filePicker.FileTypeFilter.Add(".png");
                    filePicker.FileTypeFilter.Add(".jpeg");
                    filePicker.ViewMode = PickerViewMode.Thumbnail;
                    StorageFile file = await filePicker.PickSingleFileAsync();
                    if (file == null)
                    {
                        return;
                    }
                    await LoadFileAsync(file);
                });
            }
        }

        public async Task LoadFileAsync(StorageFile file)
        {
            Stream stream = await file.OpenStreamForReadAsync();
            var properties = await file.Properties.GetImagePropertiesAsync();
            await SetCanvas(properties.Width, properties.Height, stream.AsRandomAccessStream());
        }
        public async Task SetCanvas(double width, double height, IRandomAccessStream randomAccessStream)
        {
            CanvasVisibility = Visibility.Visible;

            canvasControl.Height = height;
            canvasControl.Width = width;

            canvasBitmap = await CanvasBitmap.LoadAsync(canvasControl, randomAccessStream);
            //CanvasDevice device = CanvasDevice.GetSharedDevice();
            //CanvasRenderTarget offscreen = new CanvasRenderTarget(device, (float)width/2,(float)height/2,96);
            if (PictureCompression.UsePictureCompression)
            {
                ScaleEffect effect = new ScaleEffect()
                {
                    Source = canvasBitmap,
                    Scale = new Vector2((float)(160 / width))
                };
                CanvasDevice device = CanvasDevice.GetSharedDevice();
                CanvasRenderTarget offscreen = new CanvasRenderTarget(device, (float)160, (float)(height * 160 / width), 96);
                using (CanvasDrawingSession ds = offscreen.CreateDrawingSession())
                {
                    ds.Clear(Colors.Black);
                    ds.DrawImage(effect);
                }
                canvasControl.Height = offscreen.Size.Height;
                canvasControl.Width = offscreen.Size.Width;
                canvasBitmap = offscreen;
            }
            canvasControl.Invalidate();
            ASCIIText = string.Empty;
            BitmapImage = new BitmapImage();
        }

        public TappedEventHandler ConvertASCII
        {
            get
            {
                return async (s, e) =>
                {
                    CanvasVisibility = Visibility.Collapsed;
                    ASCIIText = GetPixels();
                    if (BackgroundImageService.UseBackgroundImage)
                    {
                        using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                        {
                            await canvasBitmap.SaveAsync(stream, CanvasBitmapFileFormat.Png);
                            BitmapImage.SetSource(stream);
                        }
                    }

                    canvasBitmap.Dispose();
                    canvasBitmap = null;
                };
            }
        }

        private int TextWidth, TextHeight;


        public string GetPixels()
        {
            //对比度
            double contrastFactor = (259.0 * (contrast + 255.0)) / (255.0 * (259.0 - contrast));
            char[] _characters = CharacterService.Characters.ToCharArray();

            StringBuilder builder = new StringBuilder();

            double width = canvasBitmap.Size.Width;
            double height = canvasBitmap.Size.Height;

            var pixels = canvasBitmap.GetPixelColors();
            int pixelDensity = PixelDensity;
            int x = 0, y = 0;
            for (y = 0; y < height; y += pixelDensity * 2)
            {
                for (x = 0; x < width; x += pixelDensity)
                {
                    int offset = (int)(y * width + x);
                    if (offset >= pixels.Length)
                    {
                        break;
                    }
                    var color = pixels[offset];
                    Color contrastedcolor = new Color()
                    {
                        A = color.A,
                        R = bound((int)(Math.Floor((color.R - 128) * contrastFactor + 128)), new int[] { 0, 255 }),
                        G = bound((int)(Math.Floor((color.G - 128) * contrastFactor + 128)), new int[] { 0, 255 }),
                        B = bound((int)(Math.Floor((color.B - 128) * contrastFactor + 128)), new int[] { 0, 255 }),
                    };
                    var bright = (0.299 * contrastedcolor.R + 0.586 * contrastedcolor.G + 0.114 * contrastedcolor.B) / 255;
                    //var character = Characters.characters[(Characters.characters.Length - 1) - (int)Math.Round(bright * (Characters.characters.Length - 1))];
                    var character = _characters[(_characters.Length - 1) - (int)Math.Round(bright * (_characters.Length - 1))];
                    builder.Append(character);
                }
                builder.Append("\r\n");
            }
            TextWidth = x / pixelDensity;
            TextHeight = y / (2 * pixelDensity);
            return builder.ToString();
        }

        byte bound(int value, int[] interval)
        {
            int i = Math.Max(interval[0], Math.Min(interval[1], value));
            if (i >= 0 && i <= 255)
            {
                return (byte)i;
            }
            else
            {
                return 255;
            }
        }

        private ICommand _getStorageItemsCommand;
        public ICommand GetStorageItemsCommand => _getStorageItemsCommand ?? (_getStorageItemsCommand = new RelayCommand<IReadOnlyList<IStorageItem>>(OnGetStorageItem));
        public async void OnGetStorageItem(IReadOnlyList<IStorageItem> items)
        {
            if (items.Count > 0)
            {
                if (items[0] != null)
                {
                    var file = items[0] as StorageFile;
                    if (file.FileType.ToLower().Equals(".jpg") || file.FileType.ToLower().Equals(".png") || file.FileType.ToLower().Equals(".jpeg"))
                    {
                        await LoadFileAsync(file);
                    }
                }
            }
        }

        public async void PasteImage()
        {
            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Bitmap))
            {
                IRandomAccessStreamReference imageReceived = null;
                try
                {
                    imageReceived = await dataPackageView.GetBitmapAsync();
                }
                catch (Exception ex)
                {
                }
                if (imageReceived != null)
                {
                    using (var imageStream = await imageReceived.OpenReadAsync())
                    {
                        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                        await SetCanvas(decoder.PixelWidth, decoder.PixelHeight, imageStream);
                    }
                }
            }
        }

        public ICommand CameraUIImage
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    CameraCaptureUI captureUI = new CameraCaptureUI();
                    captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
                    captureUI.PhotoSettings.CroppedSizeInPixels = new Size(150, 150);
                    StorageFile photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
                    if (photo != null)
                    {
                        await LoadFileAsync(photo);
                    }
                });
            }
        }
        public ICommand DirectCapture
        {
            get
            {
                return new RelayCommand(async () =>
                {
                    using (MediaCapture mediaCapture = new MediaCapture())
                    {
                        try
                        {
                            //MediaCaptureInitializationSettings mediaCaptureInitializationSettings = new MediaCaptureInitializationSettings();                            
                            await mediaCapture.InitializeAsync();
                        }
                        catch (Exception)
                        {
                            AccessDialog accessDialog = new AccessDialog();
                            await accessDialog.ShowAsync();
                            return;
                        }
                        using (var captureStream = new InMemoryRandomAccessStream())
                        {
                            await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                            using (var stream = new InMemoryRandomAccessStream())
                            {
                                var decoder = await BitmapDecoder.CreateAsync(captureStream);
                                var encoder = await BitmapEncoder.CreateForTranscodingAsync(stream, decoder);
                                var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }};
                                await encoder.BitmapProperties.SetPropertiesAsync(properties);
                                await encoder.FlushAsync();
                                await SetCanvas(decoder.PixelWidth, decoder.PixelHeight, stream);
                            }
                        }
                        //var lowLagCapture = await mediaCapture.PrepareLowLagPhotoCaptureAsync(ImageEncodingProperties.CreateUncompressed(MediaPixelFormat));
                        //var capturedPhoto = await lowLagCapture.CaptureAsync();
                        //var softwareBitmapFrame = capturedPhoto.Frame;                        

                        //await SetCanvas(softwareBitmapFrame.Width, softwareBitmapFrame.Height, softwareBitmapFrame.CloneStream());
                        //await lowLagCapture.FinishAsync();
                    }
                });
            }
        }

        private bool isPress = false;

        public PointerEventHandler PointerPressedEventHandler
        {
            get
            {
                return (s, e) =>
                {
                    isPress = true;
                    DrawPixels(s as TextBlock, e);
                };
            }
        }
        public PointerEventHandler PointerReleaseEventHandler
        {
            get
            {
                return (s, e) =>
                {
                    isPress = false;
                };
            }
        }
        public PointerEventHandler PointerCancelEventHandler
        {
            get
            {
                return (s, e) =>
                {
                    isPress = false;
                };
            }
        }
        public PointerEventHandler PointerExitEventHandler
        {
            get
            {
                return (s, e) =>
                {
                    isPress = false;
                };
            }
        }

        public PointerEventHandler PointerMoveEventHandler
        {
            get
            {
                return (s, e) =>
                {
                    if (isPress)
                    {
                        DrawPixels(s as TextBlock, e);
                    }
                };
            }
        }

        private void DrawPixels(TextBlock s,PointerRoutedEventArgs e)
        {
            double width = s.ActualWidth;
            double height = s.ActualHeight;
            var position = e.GetCurrentPoint(s).Position;
            int trueWidth = (int)Math.Round((position.X / width) * TextWidth);
            int trueHeight = (int)Math.Round((position.Y / height) * TextHeight);

            int length = trueHeight * (TextWidth + 2) + trueWidth;
            //Debug.WriteLine($"X:{trueWidth}|Y:{trueHeight}|length:{length}");
            if (length < ASCIIText.Length - 1)
            {
                StringBuilder sb = new StringBuilder(ASCIIText);
                char txt = ASCIIText[length];
                sb.Replace(ASCIIText[length], PaintingCharacter[0], length, 1);
                ASCIIText = sb.ToString();
            }
        }
    }
}
