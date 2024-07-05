using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Core.DataShop.ImageLoader
{
    public class AsyncBitmapImage
    {
        public string ImageUri { get; set; }
        public BitmapImage ImageSource { get; set; }
        public BitmapImage RawImageSource { get; set; }

        public async Task LoadImageAsync(int decodeWidth, bool cache)
        {
            ImageSource = await ImageCourier.GetImageAsync(ImageUri, decodeWidth, 0, cache);
        }

        public async Task LoadRawImageAsync(bool cache)
        {
            RawImageSource = await ImageCourier.GetImageAsync(ImageUri, 0, 0, cache);
        }
    }
}
