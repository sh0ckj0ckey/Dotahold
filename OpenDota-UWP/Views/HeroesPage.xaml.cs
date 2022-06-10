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
    public sealed partial class HeroesPage : Page
    {
        DotaHeroesViewModel ViewModel = null;
        DotaViewModel MainViewModel = null;

        public static DotaHeroes SelectedHero;

        public HeroesPage()
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                if (e.Parameter is NavigationTransitionInfo transition)
                {
                    navigationTransition.DefaultNavigationTransitionInfo = transition;
                }

                //判断是否需要下载新的数据，不用的话直接从DotaHeroHelper._data即可访问整个json，需要的话调用下载方法
                //await APIHelper.DownloadHeroAttributesDataAsync();

                base.OnNavigatedTo(e);
            }
            catch { }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.iHeroAttrTabIndex = 0;
            }
            catch { }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.iHeroAttrTabIndex = 1;
            }
            catch { }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.iHeroAttrTabIndex = 2;
            }
            catch { }
        }
    }
}
