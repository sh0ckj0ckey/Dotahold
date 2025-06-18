using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Dotahold.Utils;

namespace Dotahold.ViewModels
{
    internal partial class ConnectViewModel
    {
        private readonly SerialTaskQueue _serialTaskQueue = new();

        /// <summary>
        /// List of player connect records, used to show the last 3 players who connected to the game
        /// </summary>
        public readonly ObservableCollection<PlayerConnectRecordModel> PlayerConnectRecords = [];

        public async Task LoadPlayerConnectRecords()
        {
            try
            {
                string json = await StorageFilesCourier.ReadFileAsync("dotaid_bind_history");

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
                    _ = _serialTaskQueue.EnqueueAsync(() => recordModel.AvatarImage.LoadImageAsync());
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
                _ = _serialTaskQueue.EnqueueAsync(() => recordModel.AvatarImage.LoadImageAsync());
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
                await StorageFilesCourier.WriteFileAsync("dotaid_bind_history", json);
            }
            catch (Exception ex) { LogCourier.Log($"SavePlayerConnectRecords error: {ex.Message}", LogCourier.LogType.Error); }
        }

    }
}
