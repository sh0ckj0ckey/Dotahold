using System.Collections.ObjectModel;
using System.Linq;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Dotahold.ViewModels;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Items
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ItemsPage : Page
    {
        private MainViewModel? _viewModel;

        private readonly ObservableCollection<ItemModel> _components = [];

        public ItemsPage()
        {
            this.InitializeComponent();

            this.Loaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
            };

            this.Unloaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModel = e.Parameter as MainViewModel;
        }

        private bool TryGoBack()
        {
            if (_viewModel?.ItemsViewModel.SelectedItem is not null)
            {
                _viewModel.ItemsViewModel.SelectedItem = null;
                HideItemInfoStoryboard?.Begin();
            }

            return true;
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                && e.VirtualKey == VirtualKey.Left
                && e.KeyStatus.IsMenuKeyDown == true
                && !e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void System_BackRequested(object? sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
            {
                e.Handled = TryGoBack();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox)?.Text ?? string.Empty;
            _viewModel?.ItemsViewModel?.FilterItems(searchText);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_viewModel is null)
                {
                    return;
                }

                if (e.AddedItems?.FirstOrDefault() is ItemModel addedItem && addedItem is not null)
                {
                    if (_viewModel.ItemsViewModel.SelectedItem != addedItem)
                    {
                        _viewModel.ItemsViewModel.SelectedItem = addedItem;
                        ShowItemInfoStoryboard?.Begin();
                    }
                }
                else if (e.RemovedItems?.FirstOrDefault() is ItemModel removedItem && removedItem is not null)
                {
                    if (_viewModel.ItemsViewModel.SelectedItem == removedItem)
                    {
                        _viewModel.ItemsViewModel.SelectedItem = null;
                        HideItemInfoStoryboard?.Begin();
                    }
                }

                // Update components list
                _components.Clear();
                if (_viewModel.ItemsViewModel.SelectedItem is not null)
                {
                    if (_viewModel.ItemsViewModel.SelectedItem.DotaItemAttributes.components?.Length > 0)
                    {
                        foreach (var itemKey in _viewModel.ItemsViewModel.SelectedItem.DotaItemAttributes.components!)
                        {
                            var item = _viewModel.ItemsViewModel.GetItemModel(itemKey);
                            if (item is not null)
                            {
                                _components.Add(item);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }
        }

        private void ComponentsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (_viewModel is null)
                {
                    return;
                }

                if (e.ClickedItem is ItemModel item)
                {
                    _viewModel.ItemsViewModel.SelectedItem = item;
                    ItemsListView.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
                    ShowItemInfoStoryboard?.Begin();
                }
            }
            catch (System.Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }
        }
    }
}
