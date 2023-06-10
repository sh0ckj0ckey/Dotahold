using Dotahold.Core.DataShop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        private string _appVersion = string.Empty;
        private long _lastTimeCleanCache = 0;
        public SettingPage()
        {
            this.InitializeComponent();
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
        /// 切换黑白模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppearanceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = 0;// SettingsService.SelectedIndex;
                if (index == 1)
                {
                    Switch2LightMode();
                }
                else if (index == 0)
                {
                    Switch2DarkMode();
                }
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
                //ViewModel.eAppTheme = ElementTheme.Light;

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
                //ViewModel.eAppTheme = ElementTheme.Dark;

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
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold"));
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
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/sh0ckj0ckey/Dotahold/issues"));
            }
            catch { }
        }

        /// <summary>
        /// 选择启动页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = 0;// StartupPageComboBox.SelectedIndex;
                //if (ViewModel.iStartupTabIndex != index)
                //{
                //    App.AppSettingContainer.Values["StartupPage"] = index;
                //    ViewModel.iStartupTabIndex = index;
                //}
            }
            catch { }
        }

        /// <summary>
        /// 选择语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                int index = 0;// LanguageComboBox.SelectedIndex;
                //if (ViewModel.iLanguageIndex != index)
                //{
                //    App.AppSettingContainer.Values["Language"] = index;
                //    ViewModel.iLanguageIndex = index;
                //}
            }
            catch { }
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickClearImageCache(object sender, RoutedEventArgs e)
        {
            try
            {
                //ViewModel.ClearImageCache();
            }
            catch { }
        }


        private async void OnClickOpenDataDir(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await StorageFilesCourier.GetDataFolder();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch { }
        }

        private async void OnClickOpenImageCacheDir(object sender, RoutedEventArgs e)
        {
            try
            {
                var folder = await ImageCourier.GetCacheFolderAsync();
                await Launcher.LaunchFolderAsync(folder);
            }
            catch { }
        }

        private void OnClickForceUpdateLocalJson(object sender, RoutedEventArgs e)
        {
            try
            {
                ConstantsCourier.Instance.ResetConstantsGottenDate();
            }
            catch { }
        }

        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            try
            {
               // ViewModel.bShowDevTools = true;
            }
            catch { }
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void OnClickGoToStoreRate(object sender, RoutedEventArgs e)
        {

        }
    }
}
