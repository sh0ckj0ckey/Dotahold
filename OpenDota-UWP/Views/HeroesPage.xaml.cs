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

        /// <summary>
        /// 三种属性的英雄根据情况放到这个集合里面
        /// </summary>
        public ObservableCollection<DotaHeroes> HeroesObservableCollection = new ObservableCollection<DotaHeroes>();

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
            if (e.Parameter is NavigationTransitionInfo transition)
            {
                navigationTransition.DefaultNavigationTransitionInfo = transition;
            }

            //判断是否需要下载新的数据，不用的话直接从DotaHeroHelper._data即可访问整个json，需要的话调用下载方法
            //await APIHelper.DownloadHeroAttributesDataAsync();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (HeroesObservableCollection != null)
            {
                HeroesObservableCollection.Clear();
                HeroesObservableCollection = null;
            }

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// 弹出对话框
        /// </summary>
        /// <param name="content"></param>
        /// <param name="title"></param>
        public async void ShowDialog(string content)
        {
            var dialog = new ContentDialog()
            {
                Title = ":(",
                Content = content,
                PrimaryButtonText = "好的",
                FullSizeDesired = false,
            };

            dialog.PrimaryButtonClick += (_s, _e) => { };
            try
            {
                await dialog.ShowAsync();
            }
            catch { }
        }

        private void Rectangle_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Rectangle_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }
    }
}
