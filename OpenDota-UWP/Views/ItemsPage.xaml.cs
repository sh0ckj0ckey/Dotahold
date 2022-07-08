using OpenDota_UWP.Helpers;
using OpenDota_UWP.Models;
using OpenDota_UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class ItemsPage : Page
    {
        DotaItemsViewModel ViewModel = null;
        DotaViewModel MainViewModel = null;

        public ItemsPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaItemsViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;

                FrameShadow.Receivers.Add(ItemsListGrid);
                ItemGrid.Translation += new System.Numerics.Vector3(0, 0, 36);

                ItemFrame.Navigate(typeof(BlankPage));
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
                base.OnNavigatedTo(e);

                if (e.Parameter is NavigationTransitionInfo transition)
                {
                    navigationTransition.DefaultNavigationTransitionInfo = transition;
                }

                ViewModel?.LoadDotaItems();
            }
            catch { }
        }

        /// <summary>
        /// 点击列表显示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.ClickedItem is Models.DotaItemModel item)
                {
                    ViewModel.CurrentItem = item;
                    ItemFrame.Navigate(typeof(ItemInfoPage));
                }
            }
            catch { }
        }

        /// <summary>
        /// Unicode转换汉字
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private string UnicodeToString(string text)
        {
            System.Text.RegularExpressions.MatchCollection mc = System.Text.RegularExpressions.Regex.Matches(text, "\\\\u([\\w]{4})");
            if (mc != null && mc.Count > 0)
            {
                foreach (System.Text.RegularExpressions.Match m2 in mc)
                {
                    string v = m2.Value;
                    string word = v.Substring(2);
                    byte[] codes = new byte[2];
                    int code = Convert.ToInt32(word.Substring(0, 2), 16);
                    int code2 = Convert.ToInt32(word.Substring(2), 16);
                    codes[0] = (byte)code2;
                    codes[1] = (byte)code;
                    text = text.Replace(v, System.Text.Encoding.Unicode.GetString(codes));
                }
            }
            return text;
        }
    }
}
