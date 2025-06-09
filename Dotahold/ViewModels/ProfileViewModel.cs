using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class ProfileViewModel(HeroesViewModel heroesViewModel, ItemsViewModel itemsViewModel) : ObservableObject
    {
        private CancellationTokenSource? _cancellationTokenSource;

        private readonly HeroesViewModel _heroesViewModel = heroesViewModel;

        private readonly ItemsViewModel _itemsViewModel = itemsViewModel;

        private bool _loadingHeroesAndItems = false;

        private bool _loadingPlayerProfile;

        private bool _loadingPlayerWinLose = false;

        private bool _loadingPlayerOverallPerformance = false;

        private bool _loadingPlayerHeroesPerformance = false;

        private PlayerProfileModel? _playerProfile;

        private PlayerWinLoseModel? _playerWinLose;

        private int _currentPlayersNumber = 0;

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
        /// Indicates whether is currently fetching the player's overall performance data
        /// </summary>
        public bool LoadingPlayerOverallPerformance
        {
            get => _loadingPlayerOverallPerformance;
            set => SetProperty(ref _loadingPlayerOverallPerformance, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's heroes performance data
        /// </summary>
        public bool LoadingPlayerHeroesPerformance
        {
            get => _loadingPlayerHeroesPerformance;
            set => SetProperty(ref _loadingPlayerHeroesPerformance, value);
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
        /// List of player overall performance data
        /// </summary>
        public readonly ObservableCollection<PlayerOverallPerformanceModel> PlayerOverallPerformances = [];

        /// <summary>
        /// List of player hero performance, used to show the top 10 heroes played by the player
        /// </summary>
        public readonly ObservableCollection<PlayerHeroPerformanceModel> PlayerHeroPerformancesTop10 = [];

        /// <summary>
        /// List of player hero performance, used to show the performance of each hero played by the player
        /// </summary>
        public readonly ObservableCollection<PlayerHeroPerformanceModel> PlayerHeroPerformances = [];

        /// <summary>
        /// Current number of players in the game
        /// </summary>
        public int CurrentPlayersNumber
        {
            get => _currentPlayersNumber;
            set => SetProperty(ref _currentPlayersNumber, value);
        }

        /// <summary>
        /// List of player connect records, used to show the last few players who connected to the game
        /// </summary>
        public readonly ObservableCollection<PlayerConnectRecordModel> PlayerConnectRecords = [];

        public async Task LoadPlayerOverview(string steamId)
        {
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource?.Token ?? CancellationToken.None;

                this.LoadingHeroesAndItems = true;

                await _heroesViewModel.LoadHeroes();
                await _heroesViewModel.LoadAbilities();
                await _itemsViewModel.LoadItems();

                this.LoadingHeroesAndItems = false;

                var profileTask = LoadPlayerProfile(steamId, cancellationToken);
                var winLoseTask = LoadPlayerWinLose(steamId, cancellationToken);
                var overallPerformanceTask = LoadPlayerOverallPerformance(steamId, cancellationToken);
                var heroesPerformanceTask = LoadPlayerHeroesPerformance(steamId, cancellationToken);
                var currentPlayersTask = LoadCurrentPlayersNumber();

                await Task.WhenAll(profileTask, winLoseTask, overallPerformanceTask, heroesPerformanceTask, currentPlayersTask);
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerOverview({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task LoadPlayerProfile(string steamId, CancellationToken cancellationToken)
        {
            try
            {
                this.LoadingProfile = true;
                this.PlayerProfile = null;

                var profile = await ApiCourier.GetPlayerProfile(steamId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (profile?.profile is not null)
                {
                    this.PlayerProfile = new PlayerProfileModel(profile);
                    _ = this.PlayerProfile.AvatarImage.LoadImageAsync();
                }

                this.LoadingProfile = false;
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerProfile({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task LoadPlayerWinLose(string steamId, CancellationToken cancellationToken)
        {
            try
            {
                this.LoadingPlayerWinLose = true;
                this.PlayerWinLose = null;

                var winLose = await ApiCourier.GetPlayerWinLose(steamId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (winLose is not null)
                {
                    this.PlayerWinLose = new PlayerWinLoseModel(winLose);
                }

                this.LoadingPlayerWinLose = false;
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerWinLose({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task LoadPlayerOverallPerformance(string steamId, CancellationToken cancellationToken)
        {
            try
            {
                this.LoadingPlayerOverallPerformance = true;
                this.PlayerOverallPerformances.Clear();

                var overallPerformances = await ApiCourier.GetPlayerOverallPerformances(steamId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (overallPerformances is not null)
                {
                    double kills = -1, deaths = -1, assists = -1;

                    foreach (var performance in overallPerformances)
                    {
                        if (performance.field == "kills") kills = performance.n;
                        if (performance.field == "deaths") deaths = performance.n;
                        if (performance.field == "assists") assists = performance.n;
                    }

                    if (kills >= 0 && deaths >= 0 && assists >= 0)
                    {
                        this.PlayerOverallPerformances.Add(new PlayerOverallPerformanceModel(new Data.Models.DotaPlayerOverallPerformanceModel
                        {
                            field = "KDA",
                            n = deaths,
                            sum = kills + assists,
                        }));
                    }

                    foreach (var performance in overallPerformances)
                    {
                        if (PlayerOverallPerformanceModel.IsFieldAvailable(performance.field))
                        {
                            this.PlayerOverallPerformances.Add(new PlayerOverallPerformanceModel(performance));
                        }
                    }
                }

                this.LoadingPlayerOverallPerformance = false;
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerOverallPerformance({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task LoadPlayerHeroesPerformance(string steamId, CancellationToken cancellationToken)
        {
            try
            {
                this.LoadingPlayerHeroesPerformance = true;
                this.PlayerHeroPerformances.Clear();

                var heroPerformances = await ApiCourier.GetPlayerHeroPerformances(steamId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (heroPerformances is not null)
                {
                    foreach (var heroPerformance in heroPerformances)
                    {
                        var hero = _heroesViewModel.GetHeroById(heroPerformance.hero_id.ToString());
                        var performanceModel = new PlayerHeroPerformanceModel(heroPerformance, hero);
                        this.PlayerHeroPerformances.Add(performanceModel);

                        if (this.PlayerHeroPerformancesTop10.Count < 10)
                        {
                            this.PlayerHeroPerformancesTop10.Add(performanceModel);
                        }
                    }
                }

                this.LoadingPlayerHeroesPerformance = false;
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerHeroesPerformance({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
        }

        private async Task LoadCurrentPlayersNumber()
        {
            try
            {
                this.CurrentPlayersNumber = 0;

                var number = await ApiCourier.GetNumberOfCurrentPlayers();


                if (number > 0)
                {
                    this.CurrentPlayersNumber = number;
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadCurrentPlayersNumber error: {ex.Message}", LogCourier.LogType.Error); }
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
