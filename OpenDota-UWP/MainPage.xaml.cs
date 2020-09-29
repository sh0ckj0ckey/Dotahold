using OpenDota_UWP.Helpers;
using OpenDota_UWP.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace OpenDota_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
        public static ApplicationDataContainer SaveContainer = ApplicationData.Current.LocalSettings;

        public MainPage()
        {
            this.InitializeComponent();

            //初次启动显示更新日志
            if (SaveContainer.Values["first"] == null || SaveContainer.Values["first"].ToString() == "yes")
            {
                AboutGrid.Visibility = Visibility.Visible;
                SettingGridPopIn.Begin();
                AboutPivot.SelectedIndex = 1;
                SaveContainer.Values["first"] = "no";
            }

            //自动切换主题
            if (SaveContainer.Values["theme"] == null || SaveContainer.Values["theme"].ToString() == "dark")
            {
                this.RequestedTheme = ElementTheme.Dark;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                DarkRadioButton.IsChecked = true;
            }
            else
            {
                this.RequestedTheme = ElementTheme.Light;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
                LightRadioButton.IsChecked = true;
            }

            //设置标题栏样式
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = new Color() { A = 0, R = 39, G = 40, B = 57 };
            titleBar.ButtonInactiveBackgroundColor = new Color() { A = 0, R = 39, G = 40, B = 57 };
            Window.Current.SetTitleBar(RealTitleGrid);

            // SetTile(DotaMatchHelper.GetSteamID());

            Current = this;


            if (NetworkCheckHelper.CheckNetwork() == false)
            {
                //断网
                MainFrame.Navigate(typeof(NoNetworkPage));
            }
            else
            {
                MainFrame.Navigate(typeof(HeroesPage), 1);
            }
        }

        /// <summary>
        /// 英雄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            HeroRectangle.Width = 3;
            ItemRectangle.Width = 0;
            MatchRectangle.Width = 0;

            if (NetworkCheckHelper.CheckNetwork() == false)
            {
                //断网
                MainFrame.Navigate(typeof(NoNetworkPage));
            }
            else
            {
                MainFrame.Navigate(typeof(HeroesPage), 1);
            }
        }

        /// <summary>
        /// 物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            HeroRectangle.Width = 0;
            ItemRectangle.Width = 3;
            MatchRectangle.Width = 0;

            if (NetworkCheckHelper.CheckNetwork() == false)
            {
                //断网
                MainFrame.Navigate(typeof(NoNetworkPage));
            }
            else
            {
                MainFrame.Navigate(typeof(ItemsPage));
            }
        }

        /// <summary>
        /// 比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            HeroRectangle.Width = 0;
            ItemRectangle.Width = 0;
            MatchRectangle.Width = 3;

            if (NetworkCheckHelper.CheckNetwork() == false)
            {
                //断网
                MainFrame.Navigate(typeof(NoNetworkPage));
            }
            else
            {
                MainFrame.Navigate(typeof(MatchesPage));
            }
        }

        /// <summary>
        /// 关于此应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            AboutGrid.Visibility = Visibility.Visible;
            SettingGridPopIn.Begin();
            AboutPivot.SelectedIndex = 0;
        }

        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            SettingGridPopOut.Begin();
        }

        private void SettingGridPopOut_Completed(object sender, object e)
        {
            AboutGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// 点击空白也可以关闭设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SettingGridPopOut.Begin();
        }

        /// <summary>
        /// 切换夜间主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Dark;
            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
            SaveContainer.Values["theme"] = "dark";
        }

        /// <summary>
        /// 切换白天主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            this.RequestedTheme = ElementTheme.Light;
            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
            SaveContainer.Values["theme"] = "light";
        }

        /// <summary>
        /// 访问 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/YeaMion/OpenDota-UWP"));
        }

        /// <summary>
        /// 打分评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NSKQN4V8X94"));
        }

        /// <summary>
        /// 访问 Steam 个人页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://steamcommunity.com/profiles/76561198194624815/"));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            LogHelper.ShowLogFolder();
        }
    }
}
