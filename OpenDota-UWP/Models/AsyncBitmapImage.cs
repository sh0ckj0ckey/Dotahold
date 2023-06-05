using Dotahold.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.ViewModels
{
    public class AsyncBitmapImage
    {
        public string ImageUri { get; set; }
        public BitmapImage ImageSource { get; set; }
        public BitmapImage RawImageSource { get; set; }

        public async Task LoadImageAsync(int decodeWidth)
        {
            ImageSource = await ImageLoader.LoadImageAsync(ImageUri);
            ImageSource.DecodePixelType = DecodePixelType.Logical;
            ImageSource.DecodePixelWidth = decodeWidth;
        }

        public async Task LoadRawImageAsync()
        {
            RawImageSource = await ImageLoader.LoadImageAsync(ImageUri);
        }
    }

}
