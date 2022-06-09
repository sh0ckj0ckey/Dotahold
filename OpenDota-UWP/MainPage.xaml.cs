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
        ViewModels.DotaViewModel ViewModel = null;

        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                Current = this;
                ViewModel = ViewModels.DotaViewModel.Instance;

                SettingShadow.Receivers.Add(SettingPopBackground);
                SettingPop.Translation += new System.Numerics.Vector3(0, 0, 36);
                FrameShadow.Receivers.Add(SideBarGrid);
                MainFrame.Translation += new System.Numerics.Vector3(0, 0, 36);

                SetTitleBar();

                MainFrame.Navigate(typeof(HeroesPage));
            }
            catch { }
        }

        /// <summary>
        /// 设置标题栏样式
        /// </summary>
        private void SetTitleBar()
        {
            try
            {
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonHoverBackgroundColor = new Color() { A = 33, R = 0, G = 0, B = 0 };
                titleBar.ButtonPressedBackgroundColor = new Color() { A = 55, R = 0, G = 0, B = 0 };

                if (ViewModel.eAppTheme == ElementTheme.Light)
                {
                    titleBar.ButtonForegroundColor = Colors.Black;
                    titleBar.ButtonHoverForegroundColor = Colors.Black;
                    titleBar.ButtonPressedForegroundColor = Colors.Black;
                }
                else
                {
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.ButtonHoverForegroundColor = Colors.White;
                    titleBar.ButtonPressedForegroundColor = Colors.White;
                }
            }
            catch { }
        }

        /// <summary>
        /// 英雄 Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iSideMenuTabIndex != 0)
                {
                    MainFrame.Navigate(typeof(HeroesPage));
                    ViewModel.iSideMenuTabIndex = 0;
                }
            }
            catch { }
        }

        /// <summary>
        /// 物品 Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iSideMenuTabIndex != 1)
                {
                    MainFrame.Navigate(typeof(ItemsPage));
                    ViewModel.iSideMenuTabIndex = 1;
                }
            }
            catch { }
        }

        /// <summary>
        /// 比赛 Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iSideMenuTabIndex != 2)
                {
                    MainFrame.Navigate(typeof(MatchesPage));
                    ViewModel.iSideMenuTabIndex = 2;
                }
            }
            catch { }
        }

        /// <summary>
        /// 打开设置
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

        /// <summary>
        /// 关闭设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppBarButton_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                SettingGridPopOut.Begin();
            }
            catch { }
        }

        /// <summary>
        /// 点击空白关闭设置窗口
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

        private void SettingGridPopOut_Completed(object sender, object e)
        {
            try
            {
                SettingPopGrid.Visibility = Visibility.Collapsed;
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
                Switch2DarkMode();
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
                Switch2LightMode();
            }
            catch { }
        }

        /// <summary>
        /// 切换到白天主题
        /// </summary>
        public void Switch2LightMode()
        {
            try
            {
                ViewModel.eAppTheme = ElementTheme.Light;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonPressedForegroundColor = Colors.Black;

                App.AppSettingContainer.Values["Theme"] = "Light";
            }
            catch { }
        }

        /// <summary>
        /// 切换到夜间主题
        /// </summary>
        public void Switch2DarkMode()
        {
            try
            {
                ViewModel.eAppTheme = ElementTheme.Dark;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonPressedForegroundColor = Colors.White;

                App.AppSettingContainer.Values["Theme"] = "Dark";
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

        /// <summary>
        /// 访问 GitHub Issues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/OpenDota-UWP-reborn/issues"));
            }
            catch { }
        }
    }
}
