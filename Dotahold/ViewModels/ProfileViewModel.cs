using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class ProfileViewModel(HeroesViewModel heroesViewModel, ItemsViewModel itemsViewModel) : ObservableObject
    {
        private readonly HeroesViewModel _heroesViewModel = heroesViewModel;

        private readonly ItemsViewModel _itemsViewModel = itemsViewModel;

        private bool _loadingHeroesAndItems = false;

        private bool _loadingPlayerProfile;

        private bool _loadingPlayerWinLose = false;

        private PlayerProfileModel? _playerProfile;

        private PlayerWinLoseModel? _playerWinLose;

        /// <summary>
        /// Indicates whether is currently fetching heroes and items data, overview loading will be blocked until this is done
        /// </summary>
        public bool LoadingHeroesAndItems
        {
            get => _loadingHeroesAndItems;
            set => SetProperty(ref _loadingHeroesAndItems, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's profile
        /// </summary>
        public bool LoadingProfile
        {
            get => _loadingPlayerProfile;
            set => SetProperty(ref _loadingPlayerProfile, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's win/loss data
        /// </summary>
        public bool LoadingPlayerWinLose
        {
            get => _loadingPlayerWinLose;
            set => SetProperty(ref _loadingPlayerWinLose, value);
        }

        /// <summary>
        /// Current player's profile
        /// </summary>
        public PlayerProfileModel? PlayerProfile
        {
            get => _playerProfile;
            set => SetProperty(ref _playerProfile, value);
        }

        /// <summary>
        /// Current player's win-lose data
        /// </summary>
        public PlayerWinLoseModel? PlayerWinLose
        {
            get => _playerWinLose;
            set => SetProperty(ref _playerWinLose, value);
        }

        /// <summary>
        /// Indicates whether the overview is currently loading.
        /// </summary>
        public bool IsOverviewLoading { get; set; } = false;

        /// <summary>
        /// List of player connect records, used to show the last few players who connected to the game
        /// </summary>
        public readonly ObservableCollection<PlayerConnectRecordModel> PlayerConnectRecords = [];

        public async Task LoadPlayerOverview(string steamId)
        {
            try
            {
                this.LoadingHeroesAndItems = true;

                await _heroesViewModel.LoadHeroes();
                await _itemsViewModel.LoadItems();

                this.LoadingHeroesAndItems = false;

                this.IsOverviewLoading = true;

                await Task.WhenAll([
                    LoadPlayerProfile(steamId),
                    LoadPlayerWinLose(steamId),
                ]);
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerOverview({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.IsOverviewLoading = false;
            }
        }

        private async Task LoadPlayerProfile(string steamId)
        {
            try
            {
                this.LoadingProfile = true;
                this.PlayerProfile = null;

                var profile = await ApiCourier.GetPlayerProfile(steamId);
                if (profile?.profile is not null)
                {
                    this.PlayerProfile = new PlayerProfileModel(profile);
                    _ = this.PlayerProfile.AvatarImage.LoadImageAsync();
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerProfile({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingProfile = false;
            }
        }

        private async Task LoadPlayerWinLose(string steamId)
        {
            try
            {
                this.LoadingPlayerWinLose = true;
                this.PlayerWinLose = null;

                var winLose = await ApiCourier.GetPlayerWinLose(steamId);
                if (winLose is not null)
                {
                    this.PlayerWinLose = new PlayerWinLoseModel(winLose.Item1, winLose.Item2);
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerWinLose({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingPlayerWinLose = false;
            }
        }

        #region Player Connect Records

        public async Task LoadPlayerConnectRecords()
        {
            try
            {
                string json = await StorageFilesCourier.ReadFileAsync("dotaidbindhistory");

                if (string.IsNullOrWhiteSpace(json))
                {
                    return;
                }

                var records = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.ListPlayerConnectRecord);

                if (records is null || records.Count <= 0)
                {
                    return;
                }

                this.PlayerConnectRecords.Clear();

                foreach (var record in records)
                {
                    var recordModel = new PlayerConnectRecordModel(record.SteamId, record.Avatar, record.Name);
                    this.PlayerConnectRecords.Add(recordModel);
                    _ = recordModel.AvatarImage.LoadImageAsync();
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerConnectRecords error: {ex.Message}", LogCourier.LogType.Error); }
        }

        public void RecordPlayerConnect(string avatar, string name, string steamId)
        {
            try
            {
                PlayerConnectRecordModel? removing = null;
                foreach (var item in this.PlayerConnectRecords)
                {
                    if (item.SteamId == steamId)
                    {
                        removing = item;
                        break;
                    }
                }

                if (removing is not null)
                {
                    this.PlayerConnectRecords.Remove(removing);
                }

                while (this.PlayerConnectRecords.Count > 2)
                {
                    this.PlayerConnectRecords.RemoveAt(this.PlayerConnectRecords.Count - 1);
                }

                var recordModel = new PlayerConnectRecordModel(steamId, avatar, name);
                this.PlayerConnectRecords.Insert(0, recordModel);
                _ = recordModel.AvatarImage.LoadImageAsync();

                _ = SavePlayerConnectRecords();
            }
            catch (Exception ex) { LogCourier.Log($"RecordPlayerConnect error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task SavePlayerConnectRecords()
        {
            try
            {
                List<PlayerConnectRecord> records = [];
                foreach (var item in this.PlayerConnectRecords)
                {
                    records.Add(new PlayerConnectRecord
                    {
                        SteamId = item.SteamId,
                        Avatar = item.Avatar,
                        Name = item.Name
                    });
                }

                string json = JsonSerializer.Serialize(records, SourceGenerationContext.Default.ListPlayerConnectRecord);
                await StorageFilesCourier.WriteFileAsync("dotaidbindhistory", json);
            }
            catch (Exception ex) { LogCourier.Log($"SavePlayerConnectRecords error: {ex.Message}", LogCourier.LogType.Error); }
        }

        #endregion

    }
}
