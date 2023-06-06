using Dotahold.Core.DataShop;
using Dotahold.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
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

namespace Dotahold
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ViewModels.DotaViewModel ViewModel = null;

        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = ViewModels.DotaViewModel.Instance;

                SetTitleBar();

                if (ViewModel.iStartupTabIndex == 1)
                {
                    MainFrame.Navigate(typeof(DotaItemsPage));
                    ViewModel.iSideMenuTabIndex = 1;
                }
                else if (ViewModel.iStartupTabIndex == 2)
                {
                    MainFrame.Navigate(typeof(DotaMatchesPage));
                    ViewModel.iSideMenuTabIndex = 2;
                }
                else
                {
                    MainFrame.Navigate(typeof(DotaHeroesPage));
                    ViewModel.iSideMenuTabIndex = 0;
                }
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
        /// 点击选择侧边栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSidebarButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.Tag != null)
                {
                    switch (btn.Tag.ToString())
                    {
                        case "heroes":
                            if (ViewModel.iSideMenuTabIndex != 0)
                            {
                                MainFrame.Navigate(typeof(DotaHeroesPage));
                                ViewModel.iSideMenuTabIndex = 0;
                            }
                            break;
                        case "items":
                            if (ViewModel.iSideMenuTabIndex != 1)
                            {
                                MainFrame.Navigate(typeof(DotaItemsPage));
                                ViewModel.iSideMenuTabIndex = 1;
                            }
                            break;
                        case "matches":
                            if (ViewModel.iSideMenuTabIndex != 2)
                            {
                                MainFrame.Navigate(typeof(DotaMatchesPage));
                                ViewModel.iSideMenuTabIndex = 2;
                            }
                            break;
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 打开设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickSettingsButton(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iSideMenuTabIndex != 3)
                {
                    MainFrame.Navigate(typeof(SettingPage));
                    ViewModel.iSideMenuTabIndex = 3;
                    ViewModel.GetImageCacheSize();
                }
            }
            catch { }
        }
    }
}
