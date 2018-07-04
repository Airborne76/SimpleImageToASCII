using Microsoft.Toolkit.Uwp.Notifications;
using SimpleImageToASCII.Helpers;
using Windows.UI.Notifications;

namespace SimpleImageToASCII.Services
{
    internal partial class ToastNotificationsService
    {
        public void ShowToastNotificationSample(string filename)
        {
            // Create the toast content
            var content = new ToastContent()
            {
                // More about the Launch property at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastcontent
                Launch = "ToastContentActivationParams",

                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = "ToastNotification_PictureSaved".GetLocalized()
                            },
                            //new AdaptiveText()
                            //{
                            //     Text = @"Click OK to see how activation from a toast notification can be handled in the ToastNotificationService."
                            //}
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        // More about Toast Buttons at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastbutton
                        new ToastButton("ToastNotification_Open".GetLocalized(), filename)
                        {
                            ActivationType = ToastActivationType.Foreground
                        },
                        new ToastButtonDismiss("ToastNotification_Close".GetLocalized())
                    }
                }
            };

            // Add the content to the toast
            var toast = new ToastNotification(content.GetXml())
            {
                // TODO WTS: Set a unique identifier for this notification within the notification group. (optional)
                // More details at https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification.tag
                Tag = "ToastTag"
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
