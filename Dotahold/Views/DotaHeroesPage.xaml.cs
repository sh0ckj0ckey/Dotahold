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
using Windows.UI.Text;
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
    public sealed partial class DotaHeroesPage : Page
    {
        // 用来抑制页面跳转时其他的动画的，这样可以避免其他动画和 Connected Animation 出现奇怪的冲突
        private SuppressNavigationTransitionInfo snti = new SuppressNavigationTransitionInfo();

        private DotaHeroesViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        public DotaHeroesPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaHeroesViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;
            }
            catch { }
        }
        /// <summary>
        /// 重写导航至此页面的代码,显示动画
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                base.OnNavigatedTo(e);

                if (e.Parameter is NavigationTransitionInfo transition)
                {
                    navigationTransition.DefaultNavigationTransitionInfo = transition;
                }

                bool load = await DotaHeroesViewModel.Instance?.LoadDotaHeroes();
                if (load) DotaHeroesViewModel.Instance?.LoadHeroesImages();
            }
            catch { }
        }

        /// <summary>
        /// 点击英雄头像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClickHero(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (sender is GridView collection &&
                    collection.ContainerFromItem(e.ClickedItem) is GridViewItem container &&
                    e.ClickedItem is Core.Models.DotaHeroModel hero)
                {
                    ViewModel.PickHero(hero);
                    collection.PrepareConnectedAnimation("animateHeroInfoPhoto", hero, "HeroPhotoImg");
                    Frame.Navigate(typeof(HeroInfoPage), null, snti);
                }
            }
            catch { }
        }

        /// <summary>
        /// 从详情页返回时，处理连续动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnHeroesGridViewLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is GridView gv && gv.Tag is string tag && HeroesPivot.SelectedIndex.ToString() == tag.ToString())
                {
                    HandleAnimationBackFromHeroInfo(gv, ViewModel.CurrentHero);
                }
            }
            catch { }
        }

        private async void HandleAnimationBackFromHeroInfo(GridView gv, Core.Models.DotaHeroModel item)
        {
            try
            {
                if (item != null)
                {
                    gv.ScrollIntoView(item, ScrollIntoViewAlignment.Default);
                    gv.UpdateLayout();

                    ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("animateBackHeroPhoto");
                    if (animation != null)
                    {
                        // Setup the "back" configuration if the API is present. 
                        if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
                        {
                            animation.Configuration = new DirectConnectedAnimationConfiguration();
                        }
                        await gv.TryStartConnectedAnimationAsync(animation, item, "HeroPhotoImg");
                    }

                    gv.Focus(FocusState.Programmatic);
                }
            }
            catch { }
        }
    }
}
