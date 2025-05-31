using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class ProfileViewModel : ObservableObject
    {
        private PlayerProfileModel? _playerProfile;

        /// <summary>
        /// Current player's profile
        /// </summary>
        public PlayerProfileModel? PlayerProfile
        {
            get => _playerProfile;
            set => SetProperty(ref _playerProfile, value);
        }

        public readonly ObservableCollection<PlayerConnectRecord> PlayerConnectRecords = [];

        public ProfileViewModel()
        {
            _ = LoadPlayerConnectRecords();
        }

        public async Task<PlayerProfileModel?> GetPlayerProfile(string steamId)
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
                    RecordPlayerConnect(profile.profile.avatarfull, this.PlayerProfile.DotaPlayerProfile.profile?.personaname ?? string.Empty, steamId);
                    return this.PlayerProfile;
                }
            }
            catch (Exception ex) { LogCourier.Log($"GetPlayerProfile({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }

            return null;
        }

        private void RecordPlayerConnect(string avatar, string name, string steamId)
        {
            try
            {
                PlayerConnectRecord? removing = null;
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

                this.PlayerConnectRecords.Insert(0, new PlayerConnectRecord
                {
                    SteamId = steamId,
                    Avatar = avatar,
                    Name = name,
                });

                _ = SavePlayerConnectRecords();
            }
            catch (Exception ex) { LogCourier.Log($"RecordPlayerConnect error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task LoadPlayerConnectRecords()
        {
            try
            {
                this.PlayerConnectRecords.Clear();

                string json = await StorageFilesCourier.ReadFileAsync("dotaidbindhistory");
                var records = JsonSerializer.Deserialize(json, SourceGenerationContext.Default.ListPlayerConnectRecord);

                if (records is null || records.Count <= 0)
                {
                    return;
                }

                foreach (var record in records)
                {
                    this.PlayerConnectRecords.Add(record);
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerConnectRecords error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task SavePlayerConnectRecords()
        {
            try
            {
                var records = this.PlayerConnectRecords.ToList();
                string json = JsonSerializer.Serialize(records, SourceGenerationContext.Default.ListPlayerConnectRecord);
                await StorageFilesCourier.WriteFileAsync("dotaidbindhistory", json);
            }
            catch (Exception ex) { LogCourier.Log($"SavePlayerConnectRecords error: {ex.Message}", LogCourier.LogType.Error); }
        }
    }
}
