using System;
using Windows.UI.Xaml.Controls;

namespace OpenDota_UWP.Helpers
{
    public static class DialogShower
    {
        public static async void ShowDialog(string content)
        {
            var dialog = new ContentDialog()
            {
                Title = ":(",
                Content = content,
                PrimaryButtonText = "Okay",
                FullSizeDesired = false
            };

            dialog.PrimaryButtonClick += (_s, _e) => { dialog.Hide(); };
            try
            {
                await dialog.ShowAsync();
            }
            catch { }
        }

        public static async void ShowDialog(string title, string content)
        {
            var dialog = new ContentDialog()
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "Okay",
                FullSizeDesired = false
            };

            dialog.PrimaryButtonClick += (_s, _e) => { dialog.Hide(); };
            try
            {
                await dialog.ShowAsync();
            }
            catch { }
        }
    }

}
