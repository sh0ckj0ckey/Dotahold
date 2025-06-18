using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Dotahold.Utils;

namespace Dotahold.ViewModels
{
    internal partial class ProfileViewModel(HeroesViewModel heroesViewModel, ItemsViewModel itemsViewModel, MatchesViewModel matchesViewModel) : ObservableObject
    {
        private readonly SerialTaskQueue _serialTaskQueue = new();

        private Task? _lastOverviewTask = null;

        private CancellationTokenSource? _cancellationTokenSource;

        private readonly HeroesViewModel _heroesViewModel = heroesViewModel;

        private readonly ItemsViewModel _itemsViewModel = itemsViewModel;

        private readonly MatchesViewModel _matchesViewModel = matchesViewModel;

        /// <summary>
        /// Currently loaded player's Steam ID
        /// </summary>
        private string _currentSteamId = string.Empty;

        private bool _loadingConstants = false;

        private bool _loadingOverview = false;

        private bool _loadingPlayerProfile;

        private bool _loadingPlayerWinLose = false;

        private bool _loadingPlayerOverallPerformance = false;

        private bool _loadingPlayerHeroesPerformance = false;

        private bool _loadingPlayerRecentMatches = false;

        private PlayerProfileModel? _playerProfile;

        private PlayerWinLoseModel? _playerWinLose;

        private int _currentPlayersNumber = 0;

        /// <summary>
        /// Indicates whether is currently fetching heroes and items data, overview loading will be blocked until this is done
        /// </summary>
        public bool LoadingConstants
        {
            get => _loadingConstants;
            private set => SetProperty(ref _loadingConstants, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching any player's overview data, such as profile, win/loss, overall performance, heroes performance and recent matches
        /// </summary>
        public bool LoadingOverview
        {
            get => _loadingOverview;
            private set => SetProperty(ref _loadingOverview, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's profile
        /// </summary>
        public bool LoadingPlayerProfile
        {
            get => _loadingPlayerProfile;
            private set => SetProperty(ref _loadingPlayerProfile, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's win/loss data
        /// </summary>
        public bool LoadingPlayerWinLose
        {
            get => _loadingPlayerWinLose;
            private set => SetProperty(ref _loadingPlayerWinLose, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's overall performance data
        /// </summary>
        public bool LoadingPlayerOverallPerformance
        {
            get => _loadingPlayerOverallPerformance;
            private set => SetProperty(ref _loadingPlayerOverallPerformance, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's heroes performance data
        /// </summary>
        public bool LoadingPlayerHeroesPerformance
        {
            get => _loadingPlayerHeroesPerformance;
            private set => SetProperty(ref _loadingPlayerHeroesPerformance, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching the player's recent matches data
        /// </summary>
        public bool LoadingPlayerRecentMatches
        {
            get => _loadingPlayerRecentMatches;
            private set => SetProperty(ref _loadingPlayerRecentMatches, value);
        }

        /// <summary>
        /// Current player's profile
        /// </summary>
        public PlayerProfileModel? PlayerProfile
        {
            get => _playerProfile;
            private set => SetProperty(ref _playerProfile, value);
        }

        /// <summary>
        /// Current player's win-lose data
        /// </summary>
        public PlayerWinLoseModel? PlayerWinLose
        {
            get => _playerWinLose;
            private set => SetProperty(ref _playerWinLose, value);
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
        /// List of recent matches, used to show the last 5 matches played by the player
        /// </summary>
        public readonly ObservableCollection<RecentMatchModel> RecentMatchesTop5 = [];

        /// <summary>
        /// List of recent matches, used to show the last 20 matches played by the player
        /// </summary>
        public readonly ObservableCollection<MatchModel> RecentMatches = [];

        /// <summary>
        /// Current number of players in the game
        /// </summary>
        public int CurrentPlayersNumber
        {
            get => _currentPlayersNumber;
            private set => SetProperty(ref _currentPlayersNumber, value);
        }

        /// <summary>
        /// Steam connect records
        /// </summary>
        public readonly ConnectViewModel PlayerConnectRecords = new();

        public async Task LoadPlayerOverview(string steamId)
        {
            try
            {
                if (steamId == _currentSteamId || string.IsNullOrWhiteSpace(steamId))
                {
                    return;
                }

                _currentSteamId = steamId;

                _cancellationTokenSource?.Cancel();

                if (_lastOverviewTask is not null)
                {
                    try { await _lastOverviewTask; } catch { }
                }

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;

                _lastOverviewTask = InternalLoadPlayerOverview(steamId, cancellationToken);
                await _lastOverviewTask;
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerOverview({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                _lastOverviewTask = null;
            }
        }

        private async Task InternalLoadPlayerOverview(string steamId, CancellationToken cancellationToken)
        {
            if (steamId != _currentSteamId)
            {
                return;
            }

            this.LoadingOverview = true;
            this.LoadingPlayerProfile = true;
            this.LoadingPlayerWinLose = true;
            this.LoadingPlayerOverallPerformance = true;
            this.LoadingPlayerHeroesPerformance = true;
            this.LoadingPlayerRecentMatches = true;
            this.CurrentPlayersNumber = 0;

            this.LoadingConstants = true;

            await _heroesViewModel.LoadHeroes();
            await _itemsViewModel.LoadItems();
            await _matchesViewModel.LoadAbilities();

            this.LoadingConstants = false;

            var profileTask = LoadPlayerProfile(steamId, cancellationToken);
            var winLoseTask = LoadPlayerWinLose(steamId, cancellationToken);
            var overallPerformanceTask = LoadPlayerOverallPerformance(steamId, cancellationToken);
            var heroesPerformanceTask = LoadPlayerHeroesPerformance(steamId, cancellationToken);
            var recentMatchesTask = LoadPlayerRecentMatches(steamId, cancellationToken);
            var currentPlayersTask = LoadCurrentPlayersNumber();

            await Task.WhenAll(profileTask, winLoseTask, overallPerformanceTask, heroesPerformanceTask, recentMatchesTask, currentPlayersTask);

            this.LoadingOverview = false;
        }

        /// <summary>
        /// Load player's avatar, name and rank
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task LoadPlayerProfile(string steamId, CancellationToken cancellationToken)
        {
            try
            {
                this.LoadingPlayerProfile = true;
                this.PlayerProfile = null;

                var profile = await ApiCourier.GetPlayerProfile(steamId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (profile?.profile is not null)
                {
                    this.PlayerProfile = new PlayerProfileModel(profile);
                    _ = _serialTaskQueue.EnqueueAsync(() => this.PlayerProfile.AvatarImage.LoadImageAsync());
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerProfile({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingPlayerProfile = false;
            }
        }

        /// <summary>
        /// Load player's win/loss data
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerWinLose({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingPlayerWinLose = false;
            }
        }

        /// <summary>
        /// Load player's overall performance data, such as KDA, GPM, XPM, etc.
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerOverallPerformance({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingPlayerOverallPerformance = false;
            }
        }

        /// <summary>
        /// Load player's performance data of each hero played by the player
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task LoadPlayerHeroesPerformance(string steamId, CancellationToken cancellationToken)
        {
            try
            {
                this.LoadingPlayerHeroesPerformance = true;
                this.PlayerHeroPerformances.Clear();
                this.PlayerHeroPerformancesTop10.Clear();

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
                        if (hero is null)
                        {
                            continue;
                        }

                        var performanceModel = new PlayerHeroPerformanceModel(heroPerformance, hero);
                        this.PlayerHeroPerformances.Add(performanceModel);

                        if (this.PlayerHeroPerformancesTop10.Count < 10)
                        {
                            this.PlayerHeroPerformancesTop10.Add(performanceModel);
                        }
                    }
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerHeroesPerformance({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingPlayerHeroesPerformance = false;
            }
        }

        /// <summary>
        /// Load player's recent matches
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task LoadPlayerRecentMatches(string steamId, CancellationToken cancellationToken)
        {
            try
            {
                this.LoadingPlayerRecentMatches = true;
                this.RecentMatches.Clear();
                this.RecentMatchesTop5.Clear();

                var recentMatches = await ApiCourier.GetPlayerRecentMatches(steamId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (recentMatches is not null)
                {
                    foreach (var recentMatch in recentMatches)
                    {
                        var hero = _heroesViewModel.GetHeroById(recentMatch.hero_id.ToString());
                        if (hero is null)
                        {
                            continue;
                        }

                        var abilities = _matchesViewModel.GetAbilitiesByHeroName(hero.DotaHeroAttributes.name);
                        if (abilities is null)
                        {
                            continue;
                        }

                        var abilitiesFacet = abilities.GetFacetByIndex(recentMatch.hero_variant);

                        var matchModel = new MatchModel(recentMatch, hero, abilitiesFacet);
                        this.RecentMatches.Add(matchModel);

                        if (this.RecentMatchesTop5.Count < 5)
                        {
                            var recentMatchModel = new RecentMatchModel(recentMatch, hero, abilitiesFacet);
                            this.RecentMatchesTop5.Add(recentMatchModel);
                            _ = _serialTaskQueue.EnqueueAsync(() => recentMatchModel.HeroImage.LoadImageAsync());
                        }
                    }
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerRecentMatches({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingPlayerRecentMatches = false;
            }
        }

        /// <summary>
        /// Load the current number of players in the game
        /// </summary>
        /// <returns></returns>
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

        public void Reset()
        {
            _currentSteamId = string.Empty;
        }
    }
}
