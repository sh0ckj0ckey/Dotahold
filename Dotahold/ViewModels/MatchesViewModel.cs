using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;
using Dotahold.Models;
using Dotahold.Utils;

namespace Dotahold.ViewModels
{
    internal partial class MatchesViewModel(HeroesViewModel heroesViewModel, ItemsViewModel itemsViewModel) : ObservableObject
    {
        private readonly SerialTaskQueue _serialTaskQueue = new();

        private Task? _loadAbilitiesTask = null;

        private Task? _lastMatchesTask = null;

        private Task? _lastMatchDataTask = null;

        private CancellationTokenSource? _cancellationTokenSource;

        private CancellationTokenSource? _matchDataCancellationTokenSource;

        private readonly HeroesViewModel _heroesViewModel = heroesViewModel;

        private readonly ItemsViewModel _itemsViewModel = itemsViewModel;

        /// <summary>
        /// Hero name to AbilitiesModel
        /// </summary>
        private readonly Dictionary<string, AbilitiesModel> _abilitiesModels = [];

        /// <summary>
        /// Ability Id to Ability Name
        /// </summary>
        private readonly Dictionary<string, string> _abilityIds = [];

        /// <summary>
        /// PermanentBuff Id to PermanentBuff Name
        /// </summary>
        private readonly Dictionary<string, string> _permanentBuffs = [];

        /// <summary>
        /// Currently loaded player's Steam ID
        /// </summary>
        private string _currentSteamId = string.Empty;

        /// <summary>
        /// Currently selected match ID
        /// </summary>
        private string _currentMatchId = string.Empty;

        /// <summary>
        /// Last loaded index for matches, used to load more matches incrementally
        /// </summary>
        private int _lastLoadedIndex = 0;

        /// <summary>
        /// All matches of the player
        /// </summary>
        private DotaMatchModel[]? _allMatches = null;

        private bool _loadingPlayerAllMatches = false;

        private int _matchesInTotal = 0;

        private HeroModel? _matchesHeroFilter = null;

        private bool _loadingMatchData = false;

        private MatchDataModel? _selectedMatchData = null;

        /// <summary>
        /// Indicates whether is currently fetching the player's all matches data
        /// </summary>
        public bool LoadingPlayerAllMatches
        {
            get => _loadingPlayerAllMatches;
            private set => SetProperty(ref _loadingPlayerAllMatches, value);
        }

        /// <summary>
        /// Total number of matches loaded for the current player
        /// </summary>
        public int MatchesInTotal
        {
            get => _matchesInTotal;
            private set => SetProperty(ref _matchesInTotal, value);
        }

        /// <summary>
        /// Currently applied matches filter by hero ID
        /// </summary>
        public HeroModel? MatchesHeroFilter
        {
            get => _matchesHeroFilter;
            private set => SetProperty(ref _matchesHeroFilter, value);
        }

        /// <summary>
        /// Indicates whether is currently fetching match data
        /// </summary>
        public bool LoadingMatchData
        {
            get => _loadingMatchData;
            private set => SetProperty(ref _loadingMatchData, value);
        }

        /// <summary>
        /// Match data model for the currently selected match
        /// </summary>
        public MatchDataModel? SelectedMatchData
        {
            get => _selectedMatchData;
            private set => SetProperty(ref _selectedMatchData, value);
        }

        /// <summary>
        /// Matches list, may be filtered by hero
        /// </summary>
        public ObservableCollection<MatchModel> Matches = [];

        public async Task LoadAbilities()
        {
            if (_loadAbilitiesTask is not null)
            {
                await _loadAbilitiesTask;
                return;
            }

            _loadAbilitiesTask = LoadAbilitiesInternal();

            try
            {
                await _loadAbilitiesTask;
            }
            finally
            {
                _loadAbilitiesTask = null;
            }
        }

        private async Task LoadAbilitiesInternal()
        {
            try
            {
                if (_abilitiesModels.Count <= 0)
                {
                    Dictionary<string, Data.Models.DotaAibilitiesModel> abilitiesConstant = await ConstantsCourier.GetAbilitiesConstant();

                    foreach (var abilitiesKV in abilitiesConstant)
                    {
                        var abilities = new AbilitiesModel(abilitiesKV.Value);
                        _abilitiesModels[abilitiesKV.Key] = abilities;
                    }
                }

                if (_abilityIds.Count <= 0)
                {
                    Dictionary<string, string> abilityIdsConstant = await ConstantsCourier.GetAbilityIdsConstant();

                    foreach (var abilityIdKeyValue in abilityIdsConstant)
                    {
                        _abilityIds[abilityIdKeyValue.Key] = abilityIdKeyValue.Value;
                    }
                }

                if (_permanentBuffs.Count <= 0)
                {
                    Dictionary<string, string> permanentBuffsConstant = await ConstantsCourier.GetPermanentBuffsConstant();

                    foreach (var permanentBuffKeyValue in permanentBuffsConstant)
                    {
                        _permanentBuffs[permanentBuffKeyValue.Key] = permanentBuffKeyValue.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Loading abilities failed: {ex}", LogCourier.LogType.Error);
            }
        }

        /// <summary>
        /// Get specific hero's abilities by hero name
        /// </summary>
        /// <param name="heroName"></param>
        /// <returns></returns>
        public AbilitiesModel? GetAbilitiesByHeroName(string heroName)
        {
            if (_abilitiesModels.TryGetValue(heroName, out var abilitiesModel))
            {
                return abilitiesModel;
            }

            return null;
        }

        /// <summary>
        /// Get specific ability name by id
        /// </summary>
        /// <param name="abilityId"></param>
        /// <returns></returns>
        public string GetAbilityNameById(string abilityId)
        {
            if (_abilityIds.TryGetValue(abilityId, out var abilityName))
            {
                return abilityName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Get specific permanent buff name by id
        /// </summary>
        /// <param name="permanentBuffId"></param>
        /// <returns></returns>
        public string GetPermanentBuffNameById(string permanentBuffId)
        {
            if (_permanentBuffs.TryGetValue(permanentBuffId, out var permanentBuffName))
            {
                return permanentBuffName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Load player's all matches
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task LoadPlayerAllMatches(string steamId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(steamId))
                {
                    return;
                }

                if (steamId == _currentSteamId)
                {
                    if (_lastMatchesTask is not null)
                    {
                        await _lastMatchesTask;
                    }

                    return;
                }

                _currentSteamId = steamId;

                _cancellationTokenSource?.Cancel();

                if (_lastMatchesTask is not null)
                {
                    try { await _lastMatchesTask; } catch { }
                }

                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;

                _lastMatchesTask = InternalLoadPlayerAllMatches(steamId, cancellationToken);
                await _lastMatchesTask;
            }
            catch (Exception ex) { LogCourier.Log($"LoadPlayerAllMatches({steamId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                _lastMatchesTask = null;
            }
        }

        private async Task InternalLoadPlayerAllMatches(string steamId, CancellationToken cancellationToken)
        {
            if (steamId != _currentSteamId)
            {
                return;
            }

            this.LoadingPlayerAllMatches = true;

            _allMatches = await ApiCourier.GetPlayerAllMatches(steamId, cancellationToken);

            this.LoadingPlayerAllMatches = false;
        }

        /// <summary>
        /// Show more matches filtered by Hero ID
        /// </summary>
        /// <param name="loadCount"></param>
        /// <param name="filterHeroId"></param>
        public void LoadMoreMatches(int loadCount = 20, int filterHeroId = -1)
        {
            try
            {
                if (_allMatches is null)
                {
                    throw new InvalidOperationException("No matches loaded.");
                }

                int loadedCount = 0;

                while (_lastLoadedIndex < _allMatches.Length && loadedCount < loadCount)
                {
                    var match = _allMatches[_lastLoadedIndex];
                    _lastLoadedIndex++;

                    if (filterHeroId > 0 && match.hero_id != filterHeroId)
                    {
                        continue;
                    }

                    var hero = _heroesViewModel.GetHeroById(match.hero_id);
                    if (hero is null)
                    {
                        continue;
                    }

                    var abilitiesFacet = this.GetAbilitiesByHeroName(hero.DotaHeroAttributes.name)?.GetFacetByIndex(match.hero_variant);

                    var matchModel = new MatchModel(match, hero, abilitiesFacet);
                    this.Matches.Add(matchModel);

                    loadedCount++;
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadMoreMatches error: {ex.Message}", LogCourier.LogType.Error); }
        }

        /// <summary>
        /// Sets the filter to match the specified hero.
        /// </summary>
        /// <param name="hero">The hero to filter matches by. Can be <see langword="null"/> to clear the filter.</param>
        public void SetMatchesHeroFilter(HeroModel? hero)
        {
            this.MatchesHeroFilter = hero;
        }

        /// <summary>
        /// Clear the match list when a new HeroFilter is set
        /// </summary>
        /// <param name="filterHeroId"></param>
        public async Task ClearMatches(int filterHeroId = -1)
        {
            await DispatcherExtensions.CallOnMainViewUiThreadAsync(() =>
            {
                this.Matches.Clear();
            }, Windows.UI.Core.CoreDispatcherPriority.Low);

            _lastLoadedIndex = 0;
            this.MatchesInTotal = filterHeroId == -1 ? (_allMatches?.Length ?? 0) : _allMatches?.Count(m => m.hero_id == filterHeroId) ?? 0;
        }

        /// <summary>
        /// Load match data for a specific match ID
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
        public async Task LoadMatchData(string matchId)
        {
            try
            {
                if (matchId == _currentMatchId || string.IsNullOrWhiteSpace(matchId))
                {
                    return;
                }

                _currentMatchId = matchId;

                _matchDataCancellationTokenSource?.Cancel();

                if (_lastMatchDataTask is not null)
                {
                    try { await _lastMatchDataTask; } catch { }
                }

                _matchDataCancellationTokenSource?.Dispose();
                _matchDataCancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _matchDataCancellationTokenSource.Token;

                _lastMatchDataTask = InternalLoadMatchData(matchId, cancellationToken);
                await _lastMatchDataTask;
            }
            catch (Exception ex) { LogCourier.Log($"LoadMatchData({matchId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                _lastMatchDataTask = null;
            }
        }

        private async Task InternalLoadMatchData(string matchId, CancellationToken cancellationToken)
        {
            try
            {
                if (matchId != _currentMatchId)
                {
                    return;
                }

                this.LoadingMatchData = true;
                this.SelectedMatchData = null;

                var matchData = await ApiCourier.GetMatchData(_currentMatchId, cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (matchData is not null && matchData.match_id.ToString() == _currentMatchId)
                {
                    this.SelectedMatchData = new MatchDataModel(matchData,
                        _heroesViewModel.GetHeroById,
                        _itemsViewModel.GetItemByName,
                        _itemsViewModel.GetItemById,
                        this.GetAbilitiesByHeroName,
                        this.GetAbilityNameById,
                        this.GetPermanentBuffNameById);

                    _ = _serialTaskQueue.EnqueueAsync(() => this.SelectedMatchData?.RadiantTeam?.LogoImage?.LoadImageAsync() ?? Task.CompletedTask);
                    _ = _serialTaskQueue.EnqueueAsync(() => this.SelectedMatchData?.DireTeam?.LogoImage?.LoadImageAsync() ?? Task.CompletedTask);
                }
            }
            catch (Exception ex) { LogCourier.Log($"InternalLoadMatchData({matchId}) error: {ex.Message}", LogCourier.LogType.Error); }
            finally
            {
                this.LoadingMatchData = false;
            }
        }

        public void Reset()
        {
            _currentSteamId = string.Empty;
        }
    }
}
