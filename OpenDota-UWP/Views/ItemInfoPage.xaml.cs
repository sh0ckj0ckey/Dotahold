using OpenDota_UWP.Models;
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
    public sealed partial class ItemInfoPage : Page
    {
        //DotaItems SelectedItem;
        //public ObservableCollection<DotaItems> Components = new ObservableCollection<DotaItems>();

        public ItemInfoPage()
        {
            //SelectedItem = ItemsPage.SelectedItem;
            //this.InitializeComponent();
            //InitializeComponents();
            //if (Components.Count > 0)
            //{
            //    ComponentsGridView.Header = "合成需要:";
            //}
            //else
            //{
            //    ComponentsGridView.Header = "";
            //}
            //if (SelectedItem.Info == "")
            //{
            //    InfoTextBlock.Visibility = Visibility.Collapsed;
            //}
            //if (SelectedItem.Attributes == "")
            //{
            //    AttributesTextBlock.Visibility = Visibility.Collapsed;
            //}
            //if (SelectedItem.Tips == "")
            //{
            //    TipsTextBlock.Visibility = Visibility.Collapsed;
            //}
            //if (SelectedItem.Background == "")
            //{
            //    BackgroundGrid.Visibility = Visibility.Collapsed;
            //}
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
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 初始化选择的物品的信息
        /// </summary>
        private void InitializeComponents()
        {
            //foreach (string item in SelectedItem.Components)
            //{
            //    if (item != "null")
            //    {
            //        Components.Add(new DotaItems("", item));
            //    }
            //}
        }
    }
}
