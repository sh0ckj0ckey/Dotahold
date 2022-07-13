using OpenDota_UWP.Models;
using OpenDota_UWP.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class ItemInfoPage : Page, INotifyPropertyChanged
    {
        private DotaItemsViewModel ViewModel = null;
        private DotaViewModel MainViewModel = null;

        // 当前物品的配方列表
        private ObservableCollection<Models.DotaItemModel> vComponentsList = new ObservableCollection<Models.DotaItemModel>();

        // 物品的加成信息
        private string _sAttribInfo = string.Empty;
        public string sAttribInfo
        {
            get => _sAttribInfo;
            set
            {
                if (_sAttribInfo != value)
                {
                    _sAttribInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        // 物品信息
        private string _sHintInfo = string.Empty;
        public string sHintInfo
        {
            get => _sHintInfo;
            set
            {
                if (_sHintInfo != value)
                {
                    _sHintInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        // 指示当前物品是否有配方
        private bool _bHasComponents = false;
        public bool bHasComponents
        {
            get => _bHasComponents;
            set
            {
                if (_bHasComponents != value)
                {
                    _bHasComponents = value;
                    OnPropertyChanged();
                }
            }
        }

        public ItemInfoPage()
        {
            try
            {
                this.InitializeComponent();
                ViewModel = DotaItemsViewModel.Instance;
                MainViewModel = DotaViewModel.Instance;

                //FrameShadow.Receivers.Add(ItemsListGrid);
                //ItemGrid.Translation += new System.Numerics.Vector3(0, 0, 36);
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

                try
                {
                    if (e.Parameter is NavigationTransitionInfo transition)
                    {
                        navigationTransition.DefaultNavigationTransitionInfo = transition;
                    }

                    PopInItemInfoStoryboard?.Begin();
                }
                catch { }

                try
                {
                    bHasComponents = false;
                    vComponentsList.Clear();
                    sHintInfo = string.Empty;
                    sAttribInfo = string.Empty;

                    if (DotaItemsViewModel.Instance.CurrentItem.components != null)
                    {
                        foreach (var item in DotaItemsViewModel.Instance.CurrentItem.components)
                        {
                            if (DotaItemsViewModel.Instance.mapAllItems.ContainsKey(item))
                            {
                                vComponentsList.Add(DotaItemsViewModel.Instance.mapAllItems[item]);
                            }
                        }
                    }
                    bHasComponents = vComponentsList.Count > 0;

                    StringBuilder hintSb = new StringBuilder();
                    if (DotaItemsViewModel.Instance.CurrentItem.hint != null)
                    {
                        for (int i = 0; i < DotaItemsViewModel.Instance.CurrentItem.hint.Length; i++)
                        {
                            hintSb.Append(DotaItemsViewModel.Instance.CurrentItem.hint[i]);
                            if (i < DotaItemsViewModel.Instance.CurrentItem.hint.Length - 1)
                            {
                                hintSb.Append("\n");
                            }
                        }
                    }
                    sHintInfo = hintSb.ToString();

                    StringBuilder attribSb = new StringBuilder();
                    if (DotaItemsViewModel.Instance.CurrentItem.attrib != null)
                    {
                        for (int i = 0; i < DotaItemsViewModel.Instance.CurrentItem.attrib.Length; i++)
                        {
                            var attr = DotaItemsViewModel.Instance.CurrentItem.attrib[i];
                            attribSb.Append(attr.header);
                            attribSb.Append(attr.value);
                            attribSb.Append(" ");
                            attribSb.Append(attr.footer);
                            if (i < DotaItemsViewModel.Instance.CurrentItem.attrib.Length - 1)
                            {
                                attribSb.Append("\n");
                            }
                        }
                    }
                    sAttribInfo = attribSb.ToString();

                }
                catch { }
            }
            catch { }
        }

        private void ComponentsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (sender is GridView list)
                {
                    if (e.ClickedItem is Models.DotaItemModel item)
                    {
                        ViewModel.CurrentItem = item;
                        this.Frame.Navigate(typeof(ItemInfoPage));
                    }
                }
            }
            catch { }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
