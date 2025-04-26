using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Dictionary<string, ItemModel> _itemModels { get; set; } = [];

        /// <summary>
        /// Items list
        /// </summary>
        public ObservableCollection<ItemModel> Items { get; set; } = [];

        private bool _loading = false;

        /// <summary>
        /// Indicates whether the items list is being loaded
        /// </summary>
        public bool Loading
        {
            get => _loading;
            set => SetProperty(ref _loading, value);
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
                this.Items.Clear();

                Dictionary<string, Data.Models.DotaItemModel> itemsConstant = await ConstantsCourier.GetItemsConstant();

                foreach (var item in itemsConstant.Values)
                {
                    var itemModel = new ItemModel(item);

                    this.Items.Add(itemModel);

                    _ = itemModel.ItemImage.LoadImageAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync($"Loading items failed: {ex.ToString()}", LogCourier.LogType.Error);
            }
            finally
            {
                this.Loading = false;
            }
        }
    }
}
