using Dotahold.Core.DataShop;
using Dotahold.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class DotaIdBindHistoryModel : ViewModelBase
    {
        public string PlayerName { get; set; } = string.Empty;
        public string AvatarImage { get; set; } = string.Empty;
        public string SteamId { get; set; } = string.Empty;

        [JsonIgnore]
        private BitmapImage _ImageSource = null;
        [JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            private set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (this.ImageSource != null && string.IsNullOrWhiteSpace(this.AvatarImage)) return;

                var imageSource = await ImageCourier.GetImageAsync(this.AvatarImage, decodeWidth, 0);
                if (imageSource != null)
                {
                    this.ImageSource = imageSource;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
