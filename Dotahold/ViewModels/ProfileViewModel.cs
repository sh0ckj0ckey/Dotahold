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
    internal partial class ProfileViewModel : ObservableObject
    {
        private bool _loadedPlayerConnectRecords = false;

        private PlayerProfileModel? _playerProfile;

        /// <summary>
        /// Current player's profile
        /// </summary>
        public PlayerProfileModel? PlayerProfile
        {
            get => _playerProfile;
            set => SetProperty(ref _playerProfile, value);
        }

        public readonly ObservableCollection<PlayerConnectRecordModel> PlayerConnectRecords = [];

        public async Task<bool> GetPlayerProfile(string steamId)
        {
            try
            {
                if (steamId.Length > 14)
                {
                    if (decimal.TryParse(steamId, out decimal id64))
                    {
                        steamId = (id64 - 76561197960265728).ToString();
                    }
                }

                var profile = await ApiCourier.GetPlayerProfile(steamId);
                if (profile?.profile is not null)
                {
                    this.PlayerProfile = new PlayerProfileModel(profile);
                    _ = this.PlayerProfile.AvatarImage.LoadImageAsync();
                    RecordPlayerConnect(profile.profile.avatarfull, this.PlayerProfile.DotaPlayerProfile.profile?.personaname ?? string.Empty, steamId);
                    return true;
                }
            }
            catch (Exception ex) { LogCourier.Log($"GetPlayerProfile({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }

            return false;
        }

        public async Task LoadPlayerConnectRecords()
        {
            try
            {
                if (_loadedPlayerConnectRecords)
                {
                    return;
                }

                _loadedPlayerConnectRecords = true;

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

        private void RecordPlayerConnect(string avatar, string name, string steamId)
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
    }
}
