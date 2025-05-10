using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class ItemsViewModel : ObservableObject
    {
        /// <summary>
        /// ItemId to ItemModel
        /// </summary>
        private Dictionary<string, ItemModel> _itemModels = [];

        /// <summary>
        /// All items
        /// </summary>
        private List<ItemModel> _allItems = new List<ItemModel>();

        /// <summary>
        /// Items list
        /// </summary>
        public ObservableCollection<ItemModel> Items { get; set; } = [];

        private bool _loading = false;

        private ItemModel? _selectedItem = null;

        /// <summary>
        /// Indicates whether the items list is being loaded
        /// </summary>
        public bool Loading
        {
            get => _loading;
            set => SetProperty(ref _loading, value);
        }

        /// <summary>
        /// Selected item in the list
        /// </summary>
        public ItemModel? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public async Task LoadItems()
        {
            try
            {
                if (this.Loading || _itemModels.Count > 0)
                {
                    return;
                }

                this.Loading = true;

                _itemModels.Clear();
                _allItems.Clear();
                this.Items.Clear();

                Dictionary<string, Data.Models.DotaItemModel> itemsConstant = await ConstantsCourier.GetItemsConstant();

                foreach (var item in itemsConstant)
                {
                    if (string.IsNullOrWhiteSpace(item.Key))
                    {
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(item.Value.dname))
                    {
                        continue;
                    }

                    var itemModel = new ItemModel(item.Value);

                    _allItems.Add(itemModel);
                    _itemModels[item.Key] = itemModel;
                }

                this.Loading = false;

                FilterItems(string.Empty);

                foreach (var item in _allItems)
                {
                    await item.ItemImage.LoadImageAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Loading items failed: {ex.ToString()}", LogCourier.LogType.Error);
            }
            finally
            {
                this.Loading = false;
            }
        }

        public void FilterItems(string keyword)
        {
            this.Items.Clear();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                foreach (var item in _allItems)
                {
                    if (!string.IsNullOrWhiteSpace(item.DotaItemAttributes.qual) || item.DotaItemAttributes.tier > 0)
                    {
                        this.Items.Add(item);
                    }
                }
            }
            else
            {
                string regex = ".*" + string.Join(".*", keyword.ToCharArray()) + ".*";

                foreach (var item in _allItems)
                {
                    if (Regex.IsMatch(item.DotaItemAttributes.dname!.ToLower(), regex.ToLower()))
                    {
                        this.Items.Add(item);
                    }
                }
            }
        }

        public ItemModel? GetItemModel(string itemName)
        {
            if (_itemModels.TryGetValue(itemName, out ItemModel? itemModel))
            {
                return itemModel;
            }

            return null;
        }
    }
}
