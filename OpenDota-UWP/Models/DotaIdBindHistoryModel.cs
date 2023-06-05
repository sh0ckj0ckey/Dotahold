using Dotahold.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class DotaIdBindHistoryModel : ViewModels.ViewModelBase
    {
        public string PlayerName { get; set; } = string.Empty;
        public string AvatarImage { get; set; } = string.Empty;
        public string SteamId { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        private BitmapImage _ImageSource = null;
        [Newtonsoft.Json.JsonIgnore]
        public BitmapImage ImageSource
        {
            get { return _ImageSource; }
            set { Set("ImageSource", ref _ImageSource, value); }
        }
        public async Task LoadImageAsync(int decodeWidth)
        {
            try
            {
                if (ImageSource == null && !string.IsNullOrEmpty(AvatarImage))
                {
                    ImageSource = await ImageLoader.LoadImageAsync(AvatarImage);
                    ImageSource.DecodePixelType = DecodePixelType.Logical;
                    ImageSource.DecodePixelWidth = decodeWidth;
                }
            }
            catch { }
        }
    }
}
