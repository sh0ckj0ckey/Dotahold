using OpenDota_UWP.Helpers;
using OpenDota_UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MatchInfoPage : Page
    {
        private DotaMatchesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public MatchInfoPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaMatchesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;
            }
            catch { }
        }

        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                BanPickScrollViewer?.ChangeView(0, 0, 1, true);
            }
            catch { }
        }

        /// <summary>
        /// 绘制曲线图
        /// </summary>
        //private void DrawChart(string[] adv_gold, string[] adv_xp)
        //{
        //    List<int> gold_adv = new List<int>();
        //    List<int> xp_adv = new List<int>();

        //    foreach (string item in adv_gold)
        //    {
        //        try
        //        {
        //            gold_adv.Add(Convert.ToInt32(item));
        //        }
        //        catch
        //        {
        //            gold_adv.Add(0);
        //        }
        //    }
        //    foreach (string item in adv_xp)
        //    {
        //        try
        //        {
        //            xp_adv.Add(Convert.ToInt32(item));
        //        }
        //        catch
        //        {
        //            xp_adv.Add(0);
        //        }
        //    }
        //    if (gold_adv.Count < 2 || xp_adv.Count < 2)
        //    {
        //        ChartGrid.Visibility = Visibility.Collapsed;
        //        NoChartNoteStackPanel.Visibility = Visibility.Visible;
        //        // DataChart.DataContext = this;
        //        return;
        //    }
        //    double time_gap = 560.0 / (gold_adv.Count - 1);
        //    int max = gold_adv.Max() > xp_adv.Max() ? gold_adv.Max() : xp_adv.Max();
        //    int min = gold_adv.Min() < xp_adv.Min() ? gold_adv.Min() : xp_adv.Min();
        //    GraphTextBlock1.Text = max.ToString();
        //    GraphTextBlock2.Text = ((max - (max - min) / 4) < 0 ? -(max - (max - min) / 4) : (max - (max - min) / 4)).ToString();
        //    GraphTextBlock3.Text = ((max - 2 * (max - min) / 4) < 0 ? -(max - 2 * (max - min) / 4) : (max - 2 * (max - min) / 4)).ToString();
        //    GraphTextBlock4.Text = ((max - 3 * (max - min) / 4) < 0 ? -(max - 3 * (max - min) / 4) : (max - 3 * (max - min) / 4)).ToString();
        //    GraphTextBlock5.Text = (-min).ToString();
        //    ZeroPolyline.Points = new PointCollection { new Point(0, 480 * (max - gold_adv[0]) / (max - min)), new Point(560, 480 * (max - gold_adv[0]) / (max - min)) };
        //    GraphTextBlock0.Margin = new Thickness(0, 12 + 480 * (max - gold_adv[0]) / (max - min), 8, 0);
        //    try
        //    {
        //        for (int i = 0; i < gold_adv.Count; i++)
        //        {
        //            GoldPolyline.Points.Add(new Point(time_gap * i, 480 * (max - gold_adv[i]) / (max - min)));
        //            XpPolyline.Points.Add(new Point(time_gap * i, 480 * (max - xp_adv[i]) / (max - min)));
        //        }
        //    }
        //    catch
        //    {
        //        ChartGrid.Visibility = Visibility.Collapsed;
        //        NoChartNoteStackPanel.Visibility = Visibility.Visible;
        //        return;
        //    }
        //    ChartGrid.Visibility = Visibility.Visible;
        //    NoChartNoteStackPanel.Visibility = Visibility.Collapsed;

        //    //SeriesCollection = new SeriesCollection
        //    //{
        //    //    new LineSeries
        //    //    {
        //    //        Title = "经济",
        //    //        Values = gold_adv,
        //    //        PointForeround = new SolidColorBrush(Colors.Gold),
        //    //        PointGeometrySize = 4,
        //    //        LineSmoothness = 0,
        //    //        Fill=new SolidColorBrush(new Color(){ A = 128, R = 255, G = 215, B = 0})
        //    //    },
        //    //    new LineSeries
        //    //    {
        //    //        Title = "经验",
        //    //        Values = xp_adv,
        //    //        PointForeround = new SolidColorBrush(Colors.DeepSkyBlue),
        //    //        PointGeometrySize = 4,
        //    //        LineSmoothness = 0,
        //    //        Fill=new SolidColorBrush(new Color(){ A = 128, R = 0, G = 191, B = 255})
        //    //    }
        //    //};

        //    //List<string> timeList = new List<string>();
        //    //for (int i = 0; i < gold_adv.Count; i++)
        //    //{
        //    //    timeList.Add(i.ToString());
        //    //}
        //    //Labels = timeList.ToArray();
        //    //YFormatter = value => value.ToString();
        //    //DataChart.DataContext = this;
        //}

    }
}
