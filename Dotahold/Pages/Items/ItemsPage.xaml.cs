using System.Linq;
using Dotahold.Models;
using Dotahold.ViewModels;
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

        public ItemsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _viewModel = e.Parameter as MainViewModel;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = (sender as TextBox)?.Text ?? string.Empty;
            _viewModel?.ItemsViewModel?.FilterItems(searchText);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                }
            }
            else if (e.RemovedItems?.FirstOrDefault() is ItemModel removedItem && removedItem is not null)
            {
                if (_viewModel.ItemsViewModel.SelectedItem == removedItem)
                {
                    _viewModel.ItemsViewModel.SelectedItem = null;
                }
            }
        }
    }
}
