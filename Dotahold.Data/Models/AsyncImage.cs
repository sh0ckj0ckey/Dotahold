using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Data.Models
{
    public partial class AsyncImage : ObservableObject
    {
        private bool _loaded = false;

        private BitmapImage? _image = null;

        public bool Loaded
        {
            get => _loaded;
            private set => SetProperty(ref _loaded, value);
        }

        public BitmapImage? Image
        {
            get => _image;
            private set => SetProperty(ref _image, value);
        }

        private readonly string _url;

        private readonly int _width;

        private readonly int _height;

        private readonly bool _shouldCache;

        public AsyncImage(string url, int width = 0, int height = 0, BitmapImage? defaultImage = null, bool shouldCache = true)
        {
            _url = url;
            _width = width;
            _height = height;
            _shouldCache = shouldCache;

            if (defaultImage is not null)
            {
                this.Image = defaultImage;
            }
        }

        public async Task LoadImageAsync(bool skipEmptyUrl = false)
        {
            if (this.Loaded)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_url) && skipEmptyUrl)
            {
                this.Loaded = true;
                return;
            }

            var image = await ImageCourier.GetImageAsync(_url, _width, _height, _shouldCache);

            if (image is not null)
            {
                this.Image = image;
            }

            this.Loaded = true;
        }
    }
}
