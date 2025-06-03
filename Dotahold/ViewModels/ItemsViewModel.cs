using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class ItemsViewModel : ObservableObject
    {
        /// <summary>
        /// A semaphore used to limit concurrent access to image loading operations.
        /// </summary>
        private static readonly SemaphoreSlim _imageLoadSemaphore = new(1);

        /// <summary>
        /// Task to load items, used to prevent multiple simultaneous loads
        /// </summary>
        private Task? _loadItemsTask = null;

        /// <summary>
        /// ItemId to ItemModel
        /// </summary>
        private readonly Dictionary<string, ItemModel> _itemModels = [];

        /// <summary>
        /// All items
        /// </summary>
        private readonly List<ItemModel> _allItems = [];

        /// <summary>
        /// Items list
        /// </summary>
        public ObservableCollection<ItemModel> Items { get; } = [];

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

        private static async Task SafeLoadImageAsync(Func<Task> loadImageFunc)
        {
            await _imageLoadSemaphore.WaitAsync();
            try
            {
                await loadImageFunc();
            }
            finally
            {
                _imageLoadSemaphore.Release();
            }
        }

        public async Task LoadItems()
        {
            if (_loadItemsTask is not null)
            {
                await _loadItemsTask;
                return;
            }

            _loadItemsTask = LoadItemsInternal();

            try
            {
                await _loadItemsTask;
            }
            finally
            {
                _loadItemsTask = null;
            }
        }

        private async Task LoadItemsInternal()
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
                    _ = SafeLoadImageAsync(() => item.ItemImage.LoadImageAsync());
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Loading items failed: {ex}", LogCourier.LogType.Error);
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
