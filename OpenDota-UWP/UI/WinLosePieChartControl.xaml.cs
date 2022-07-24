using Windows.UI.Xaml.Controls;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.Generic;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace OpenDota_UWP.UI
{
    public sealed partial class WinLosePieChartControl : UserControl
    {
        public WinLosePieChartControl()
        {
            this.InitializeComponent();
        }
    }
}
