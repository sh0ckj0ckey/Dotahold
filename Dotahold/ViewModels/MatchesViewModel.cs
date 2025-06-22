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

        private CancellationTokenSource? _cancellationTokenSource;

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
        /// Last loaded index for matches, used to load more matches incrementally
        /// </summary>
        private int _lastLoadedIndex = 0;

        /// <summary>
        /// All matches of the player
        /// </summary>
        private DotaMatchModel[]? _allMatches = null;

        private bool _loadingPlayerAllMatches = false;

        private HeroModel? _matchesHeroFilter = null;

        private int _matchesInTotal = 0;

        /// <summary>
        /// Indicates whether is currently fetching the player's all matches data
        /// </summary>
        public bool LoadingPlayerAllMatches
        {
            get => _loadingPlayerAllMatches;
            private set => SetProperty(ref _loadingPlayerAllMatches, value);
        }

        /// <summary>
        /// Currently applied matches filter by hero ID
        /// </summary>
        public HeroModel? MatchesHeroFilter
        {
            get => _matchesHeroFilter;
            set => SetProperty(ref _matchesHeroFilter, value);
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

        /// <summary>
        /// Load player's all matches
        /// </summary>
        /// <param name="steamId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

                    var hero = _heroesViewModel.GetHeroById(match.hero_id.ToString());
                    if (hero is null)
                    {
                        continue;
                    }

                    var abilities = this.GetAbilitiesByHeroName(hero.DotaHeroAttributes.name);
                    if (abilities is null)
                    {
                        continue;
                    }

                    var abilitiesFacet = abilities.GetFacetByIndex(match.hero_variant);

                    var matchModel = new MatchModel(match, hero, abilitiesFacet);
                    this.Matches.Add(matchModel);

                    loadedCount++;
                }
            }
            catch (Exception ex) { LogCourier.Log($"LoadMoreMatches error: {ex.Message}", LogCourier.LogType.Error); }
        }

        /// <summary>
        /// Clear the match list when a new HeroFilter is set
        /// </summary>
        /// <param name="filterHeroId"></param>
        public void ClearMatches(int filterHeroId = -1)
        {
            _lastLoadedIndex = 0;
            this.Matches.Clear();
            this.MatchesInTotal = filterHeroId == -1 ? (_allMatches?.Length ?? 0) : _allMatches?.Count(m => m.hero_id == filterHeroId) ?? 0;
        }

        public void Reset()
        {
            _currentSteamId = string.Empty;
        }
    }
}
