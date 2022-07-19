using OpenDota_UWP.Helpers;
using OpenDota_UWP.Models;
using OpenDota_UWP.ViewModels;
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

namespace OpenDota_UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HeroInfoPage : Page
    {
        private DotaHeroesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        private Style _customDialogStyle = null;

        private bool _hoveringTalentButton = false;

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

        //    double[] pick = new double[] { ConvertString2Double(heroAttr._1_pick), ConvertString2Double(heroAttr._2_pick), ConvertString2Double(heroAttr._3_pick), ConvertString2Double(heroAttr._4_pick), ConvertString2Double(heroAttr._5_pick), ConvertString2Double(heroAttr._6_pick), ConvertString2Double(heroAttr._7_pick), ConvertString2Double(heroAttr._8_pick) };
        //    double[] win = new double[] { ConvertString2Double(heroAttr._1_win), ConvertString2Double(heroAttr._2_win), ConvertString2Double(heroAttr._3_win), ConvertString2Double(heroAttr._4_win), ConvertString2Double(heroAttr._5_win), ConvertString2Double(heroAttr._6_win), ConvertString2Double(heroAttr._7_win), ConvertString2Double(heroAttr._8_win) };
        //    double max = pick.Max();
        //    double ratio = 328.0 / max;
        //    Pick_1Rectangle.Width = ConvertString2Double(heroAttr._1_pick) * ratio;
        //    Pick_2Rectangle.Width = ConvertString2Double(heroAttr._2_pick) * ratio;
        //    Pick_3Rectangle.Width = ConvertString2Double(heroAttr._3_pick) * ratio;
        //    Pick_4Rectangle.Width = ConvertString2Double(heroAttr._4_pick) * ratio;
        //    Pick_5Rectangle.Width = ConvertString2Double(heroAttr._5_pick) * ratio;
        //    Pick_6Rectangle.Width = ConvertString2Double(heroAttr._6_pick) * ratio;
        //    Pick_7Rectangle.Width = ConvertString2Double(heroAttr._7_pick) * ratio;
        //    Pick_8Rectangle.Width = ConvertString2Double(heroAttr._8_pick) * ratio;

        //    Win_1Rectangle.Width = ConvertString2Double(heroAttr._1_win) * ratio;
        //    Win_2Rectangle.Width = ConvertString2Double(heroAttr._2_win) * ratio;
        //    Win_3Rectangle.Width = ConvertString2Double(heroAttr._3_win) * ratio;
        //    Win_4Rectangle.Width = ConvertString2Double(heroAttr._4_win) * ratio;
        //    Win_5Rectangle.Width = ConvertString2Double(heroAttr._5_win) * ratio;
        //    Win_6Rectangle.Width = ConvertString2Double(heroAttr._6_win) * ratio;
        //    Win_7Rectangle.Width = ConvertString2Double(heroAttr._7_win) * ratio;
        //    Win_8Rectangle.Width = ConvertString2Double(heroAttr._8_win) * ratio;

        //    Win_1TextBlock.Text = heroAttr._1_win;
        //    Pick_1TextBlock.Text = heroAttr._1_pick;
        //    Rate_1TextBlock.Text = (100 * ConvertString2Double(heroAttr._1_win) / ConvertString2Double(heroAttr._1_pick)).ToString("f1") + "%";
        //    Win_2TextBlock.Text = heroAttr._2_win;
        //    Pick_2TextBlock.Text = heroAttr._2_pick;
        //    Rate_2TextBlock.Text = (100 * ConvertString2Double(heroAttr._2_win) / ConvertString2Double(heroAttr._2_pick)).ToString("f1") + "%";
        //    Win_3TextBlock.Text = heroAttr._3_win;
        //    Pick_3TextBlock.Text = heroAttr._3_pick;
        //    Rate_3TextBlock.Text = (100 * ConvertString2Double(heroAttr._3_win) / ConvertString2Double(heroAttr._3_pick)).ToString("f1") + "%";
        //    Win_4TextBlock.Text = heroAttr._4_win;
        //    Pick_4TextBlock.Text = heroAttr._4_pick;
        //    Rate_4TextBlock.Text = (100 * ConvertString2Double(heroAttr._4_win) / ConvertString2Double(heroAttr._4_pick)).ToString("f1") + "%";
        //    Win_5TextBlock.Text = heroAttr._5_win;
        //    Pick_5TextBlock.Text = heroAttr._5_pick;
        //    Rate_5TextBlock.Text = (100 * ConvertString2Double(heroAttr._5_win) / ConvertString2Double(heroAttr._5_pick)).ToString("f1") + "%";
        //    Win_6TextBlock.Text = heroAttr._6_win;
        //    Pick_6TextBlock.Text = heroAttr._6_pick;
        //    Rate_6TextBlock.Text = (100 * ConvertString2Double(heroAttr._6_win) / ConvertString2Double(heroAttr._6_pick)).ToString("f1") + "%";
        //    Win_7TextBlock.Text = heroAttr._7_win;
        //    Pick_7TextBlock.Text = heroAttr._7_pick;
        //    Rate_7TextBlock.Text = (100 * ConvertString2Double(heroAttr._7_win) / ConvertString2Double(heroAttr._7_pick)).ToString("f1") + "%";
        //    Win_8TextBlock.Text = heroAttr._8_win;
        //    Pick_8TextBlock.Text = heroAttr._8_pick;
        //    Rate_8TextBlock.Text = (100 * ConvertString2Double(heroAttr._8_win) / ConvertString2Double(heroAttr._8_pick)).ToString("f1") + "%";



        /// <summary>
        /// 返回按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
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
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel?.CurrentHeroInfo == null) return;

                string loc = TrimHeroHistory(ViewModel.CurrentHeroInfo.bio_loc);
                ViewModel.CurrentHeroInfo.bio_loc = loc;

                if (_customDialogStyle == null)
                {
                    _customDialogStyle = (Style)Application.Current.Resources["CustomDialogStyle"];
                }

                ContentDialog dialog = new ContentDialog();

                if (_customDialogStyle != null)
                {
                    dialog.Style = _customDialogStyle;
                }

                dialog.CloseButtonText = "Close";
                dialog.IsPrimaryButtonEnabled = false;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.Content = new UI.HeroHistoryDialogContent();
                dialog.RequestedTheme = MainViewModel.eAppTheme;

                _ = await dialog.ShowAsync();
            }
            catch { }
        }

        /// <summary>
        /// 点击查看英雄榜
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel?.CurrentHeroInfo == null) return;

                string loc = TrimHeroHistory(ViewModel.CurrentHeroInfo.bio_loc);
                ViewModel.CurrentHeroInfo.bio_loc = loc;

                if (_customDialogStyle == null)
                {
                    _customDialogStyle = (Style)Application.Current.Resources["CustomDialogStyle"];
                }

                ContentDialog dialog = new ContentDialog();

                if (_customDialogStyle != null)
                {
                    dialog.Style = _customDialogStyle;
                }

                dialog.CloseButtonText = "Close";
                dialog.IsPrimaryButtonEnabled = false;
                dialog.IsSecondaryButtonEnabled = false;
                dialog.Content = new UI.HeroPlayerRankDialogContent();
                dialog.RequestedTheme = MainViewModel.eAppTheme;

                ViewModel.FetchHeroRanking(ViewModel.CurrentHero.id);

                _ = await dialog.ShowAsync();
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

        private void HeroTalentsGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                _hoveringTalentButton = true;
                TalentsPopGrid.Visibility = Visibility.Visible;
                TalentsPopGridPopIn?.Begin();
                EnterHeroTalentStoryboard?.Begin();
            }
            catch { }
        }

        private void HeroTalentsGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                _hoveringTalentButton = false;
                TalentsPopGridPopOut?.Begin();
                ExitHeroTalentStoryboard?.Begin();
            }
            catch { }
        }

        private void TalentsPopGridPopOut_Completed(object sender, object e)
        {
            try
            {
                if (_hoveringTalentButton) return;
                TalentsPopGrid.Visibility = Visibility.Collapsed;
            }
            catch { }
        }
    }
}
