using System;
using Windows.UI.Xaml.Controls;

namespace OpenDota_UWP.Helpers
{
    public static class DialogShower
    {
        public static async void ShowDialog(string title = ":(", string content = "Something is wrong")
        {
            var dialog = new ContentDialog()
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "OK",
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
