using Dotahold.Core.DataShop;
using Dotahold.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class DotaMatchPerformCompareModel : ViewModelBase
    {
        public double LeftValue { get; set; } = 0;
        public int? LeftPlayerSlot { get; set; }

        private BitmapImage _LeftImageSource = null;
        public BitmapImage LeftImageSource
        {
            get { return _LeftImageSource; }
            set { Set("LeftImageSource", ref _LeftImageSource, value); }
        }

        public double RightValue { get; set; } = 0;
        public int? RightPlayerSlot { get; set; }

        private BitmapImage _RightImageSource = null;
        public BitmapImage RightImageSource
        {
            get { return _RightImageSource; }
            set { Set("RightImageSource", ref _RightImageSource, value); }
        }
    }
}
