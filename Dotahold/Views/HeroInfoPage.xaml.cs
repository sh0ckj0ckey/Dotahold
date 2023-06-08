using Dotahold.Models;
using Dotahold.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HeroInfoPage : Page
    {
        private DotaHeroesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public HeroInfoPage()
        {
            this.InitializeComponent();

            try
            {
                ViewModel = DotaHeroesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;

                ViewModel.ActPopInHeroInfoGrid += () =>
                {
                    try
                    {
                        PopInHeroInfoStoryboard?.Begin();
                    }
                    catch { }
                };

                ViewModel.ActShowHeroInfoButton += () =>
                {
                    try
                    {
                        ShowHeroButtonStoryboard?.Begin();
                    }
                    catch { }
                };
            }
            catch { }
        }

        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                ShowBackgroundImage?.Begin();
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("animateHeroInfoPhoto");
                if (animation != null)
                {
                    animation.TryStart(HeroPhotoBorder, new UIElement[] { HeroNameGrid });
                }
            }
            catch { }
        }

        /// <summary>
        /// 重写离开此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimation animation =
                    ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("animateBackHeroPhoto", HeroPhotoBorder);

                // Use the recommended configuration for back animation.
                animation.Configuration = new DirectConnectedAnimationConfiguration();
            }
        }

        /// <summary>
        /// 加载完成后将滚动条滚到最顶部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                HeroInfoScrollViewer?.ChangeView(0, 0, 1, true);
            }
            catch { }
        }

        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickGoBack(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Frame.CanGoBack)
                {
                    this.Frame.GoBack();
                }
            }
            catch { }
        }

        /// <summary>
        /// 点击查看英雄背景
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickHistory(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel?.CurrentHeroInfo == null) return;

                string loc = TrimHeroHistory(ViewModel.CurrentHeroInfo.bio_loc);
                ViewModel.CurrentHeroInfo.bio_loc = loc;

                ContentDialog dialog = new ContentDialog();

                dialog.CloseButtonText = "Close";
                dialog.IsPrimaryButtonEnabled = false;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.Content = new UI.HeroHistoryDialogContent();
                dialog.RequestedTheme = MainViewModel.eAppTheme;

                await dialog.ShowAsync();
            }
            catch { }
        }

        /// <summary>
        /// 点击查看英雄榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnClickPlayerRank(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel?.CurrentHeroInfo == null) return;

                string loc = TrimHeroHistory(ViewModel.CurrentHeroInfo.bio_loc);
                ViewModel.CurrentHeroInfo.bio_loc = loc;

                ContentDialog dialog = new ContentDialog();

                dialog.CloseButtonText = "Close";
                dialog.IsPrimaryButtonEnabled = false;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.Content = new UI.HeroPlayerRankDialogContent();
                dialog.RequestedTheme = MainViewModel.eAppTheme;

                ViewModel.FetchHeroRanking(ViewModel.CurrentHero.id);

                await dialog.ShowAsync();
            }
            catch { }
        }

        /// <summary>
        /// 处理英雄背景故事字符串，去掉包含的一些标签和多余的转义符
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        private string TrimHeroHistory(string history)
        {
            try
            {
                string strText = System.Text.RegularExpressions.Regex.Replace(history, "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                strText = strText.Replace("\t", "");
                strText = strText.Replace("\r", "\n");
                return strText;
            }
            catch { }
            return history;
        }

        private void OnClickHeroTalents(object sender, RoutedEventArgs e)
        {
            HeroTanlentsTeachingTip.IsOpen = true;
        }
    }
}
