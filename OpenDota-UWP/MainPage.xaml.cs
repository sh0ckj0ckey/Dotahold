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
            try
            {
                this.InitializeComponent();

                Current = this;

                SettingShadow.Receivers.Add(SettingPopBackground);
                SettingPop.Translation += new System.Numerics.Vector3(0, 0, 36);
                FrameShadow.Receivers.Add(SideBarGrid);
                MainFrame.Translation += new System.Numerics.Vector3(0, 0, 36);

                //设置标题栏样式
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = new Color() { A = 0, R = 39, G = 40, B = 57 };
                titleBar.ButtonInactiveBackgroundColor = new Color() { A = 0, R = 39, G = 40, B = 57 };


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


                MainFrame.Navigate(typeof(HeroesPage));
            }
            catch { }
        }

        /// <summary>
        /// 英雄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Navigate(typeof(HeroesPage));
            }
            catch { }
        }

        /// <summary>
        /// 物品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Navigate(typeof(ItemsPage));
            }
            catch { }
        }

        /// <summary>
        /// 比赛
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Navigate(typeof(MatchesPage));
            }
            catch { }
        }

        /// <summary>
        /// 关于此应用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingPopGrid.Visibility = Visibility.Visible;
                SettingGridPopIn.Begin();
                SettingPivot.SelectedIndex = 0;
            }
            catch { }
        }

        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingGridPopOut.Begin();
            }
            catch { }
        }

        private void SettingGridPopOut_Completed(object sender, object e)
        {
            try
            {
                SettingPopGrid.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        /// <summary>
        /// 点击空白也可以关闭设置窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rectangle_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                SettingGridPopOut.Begin();
            }
            catch { }
        }

        /// <summary>
        /// 切换夜间主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                this.RequestedTheme = ElementTheme.Dark;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.White;
                SaveContainer.Values["theme"] = "dark";
            }
            catch { }
        }

        /// <summary>
        /// 切换白天主题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            try
            {
                this.RequestedTheme = ElementTheme.Light;
                ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = Colors.Black;
                SaveContainer.Values["theme"] = "light";
            }
            catch { }
        }

        /// <summary>
        /// 访问 GitHub
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/OpenDota-UWP-reborn"));
            }
            catch { }
        }

        /// <summary>
        /// 打分评价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NSKQN4V8X94"));
            }
            catch { }
        }

        /// <summary>
        /// 访问 Steam 个人页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://steamcommunity.com/profiles/76561198194624815/"));
            }
            catch { }
        }
    }
}
