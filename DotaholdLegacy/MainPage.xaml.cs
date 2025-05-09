﻿using System;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Dotahold.Views;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace Dotahold
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ViewModels.DotaViewModel _viewModel = null;

        public MainPage()
        {
            _viewModel = DotaViewModel.Instance;

            this.InitializeComponent();

            this.Loaded += (s, e) =>
            {
                try
                {
                    SetTitleBarArea();
                    SetTitleBarTheme();

                    DotaViewModel.Instance.ActChangeTitleBarTheme = SetTitleBarTheme;

                    var index = DotaViewModel.Instance.AppSettings.StartupPageIndex;
                    if (index == 1)
                    {
                        MainFrame.Navigate(typeof(DotaItemsPage));
                        _viewModel.SideMenuTabIndex = 1;
                    }
                    else if (index == 2)
                    {
                        MainFrame.Navigate(typeof(DotaMatchesPage));
                        _viewModel.SideMenuTabIndex = 2;
                    }
                    else
                    {
                        MainFrame.Navigate(typeof(DotaHeroesPage));
                        _viewModel.SideMenuTabIndex = 0;
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
            };
        }

        #region 处理标题栏样式

        private void SetTitleBarArea()
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

            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        private void SetTitleBarTheme()
        {
            try
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;

                if (DotaViewModel.Instance.AppSettings.AppearanceIndex == 0)
                {
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.ButtonHoverForegroundColor = Colors.White;
                    titleBar.ButtonPressedForegroundColor = Colors.White;
                    titleBar.ButtonHoverBackgroundColor = new Color() { A = 16, R = 255, G = 255, B = 255 };
                    titleBar.ButtonPressedBackgroundColor = new Color() { A = 24, R = 255, G = 255, B = 255 };
                }
                else if (DotaViewModel.Instance.AppSettings.AppearanceIndex == 1)
                {
                    titleBar.ButtonForegroundColor = Colors.Black;
                    titleBar.ButtonHoverForegroundColor = Colors.Black;
                    titleBar.ButtonPressedForegroundColor = Colors.Black;
                    titleBar.ButtonHoverBackgroundColor = new Color() { A = 8, R = 0, G = 0, B = 0 };
                    titleBar.ButtonPressedBackgroundColor = new Color() { A = 16, R = 0, G = 0, B = 0 };
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }

        #endregion

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
                            if (_viewModel.SideMenuTabIndex != 0)
                            {
                                MainFrame.Navigate(typeof(DotaHeroesPage));
                                _viewModel.SideMenuTabIndex = 0;
                            }
                            break;
                        case "items":
                            if (_viewModel.SideMenuTabIndex != 1)
                            {
                                MainFrame.Navigate(typeof(DotaItemsPage));
                                _viewModel.SideMenuTabIndex = 1;
                            }
                            break;
                        case "matches":
                            if (_viewModel.SideMenuTabIndex != 2)
                            {
                                MainFrame.Navigate(typeof(DotaMatchesPage));
                                _viewModel.SideMenuTabIndex = 2;
                            }
                            break;
                        case "settings":
                            if (_viewModel.SideMenuTabIndex != 3)
                            {
                                MainFrame.Navigate(typeof(SettingPage));
                                _viewModel.SideMenuTabIndex = 3;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
