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

namespace OpenDota_UWP.Views
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

                _ = await DotaHeroesViewModel.Instance?.LoadDotaHeroes();
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iHeroAttrTabIndex == 0) return;

                ViewModel.iHeroAttrTabIndex = 0;

                SlideInStrHeroesStoryboard?.Begin();
            }
            catch { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iHeroAttrTabIndex == 1) return;

                int oldIndex = ViewModel.iHeroAttrTabIndex;
                ViewModel.iHeroAttrTabIndex = 1;

                if (oldIndex == 0)
                {
                    SlideInLeftAgiHeroesStoryboard?.Begin();
                }
                else if (oldIndex == 2)
                {
                    SlideInRightAgiHeroesStoryboard?.Begin();
                }
            }
            catch { }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iHeroAttrTabIndex == 2) return;

                ViewModel.iHeroAttrTabIndex = 2;
                SlideInIntHeroesStoryboard?.Begin();
            }
            catch { }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (sender is GridView collection &&
                    collection.ContainerFromItem(e.ClickedItem) is GridViewItem container &&
                    e.ClickedItem is Models.DotaHeroModel hero)
                {
                    ViewModel.PickHero(hero);
                    collection.PrepareConnectedAnimation("animateHeroInfoPhoto", hero, "HeroPhotoImg");
                    Frame.Navigate(typeof(HeroInfoPage), null, snti);
                }
            }
            catch { }
        }

        private void GridView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iHeroAttrTabIndex == 0 && sender is GridView gv)
                {
                    HandleAnimationBackFromHeroInfo(gv, ViewModel.CurrentHero);
                }
            }
            catch { }
        }

        private void GridView_Loaded_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iHeroAttrTabIndex == 1 && sender is GridView gv)
                {
                    HandleAnimationBackFromHeroInfo(gv, ViewModel.CurrentHero);
                }
            }
            catch { }
        }

        private void GridView_Loaded_2(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.iHeroAttrTabIndex == 2 && sender is GridView gv)
                {
                    HandleAnimationBackFromHeroInfo(gv, ViewModel.CurrentHero);
                }
            }
            catch { }
        }

        private async void HandleAnimationBackFromHeroInfo(GridView gv, Models.DotaHeroModel item)
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
