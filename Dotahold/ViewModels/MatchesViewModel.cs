using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private bool _loadingPlayerAllMatches = false;

        /// <summary>
        /// Indicates whether is currently fetching the player's all matches data
        /// </summary>
        public bool LoadingPlayerAllMatches
        {
            get => _loadingPlayerAllMatches;
            private set => SetProperty(ref _loadingPlayerAllMatches, value);
        }

        /// <summary>
        /// All matches of the player
        /// </summary>
        private DotaMatchModel[]? _allMatches = null;

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
            this.LoadingPlayerAllMatches = true;
            _allMatches = null;
            this.Matches.Clear();

            _allMatches = await ApiCourier.GetPlayerAllMatches(steamId, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            if (_allMatches is not null)
            {
                foreach (var match in _allMatches)
                {
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
                }
            }

            this.LoadingPlayerAllMatches = false;
        }

        public void Reset()
        {
            //this.LoadingPlayerAllMatches = false;
            //_allMatches = null;
            //this.Matches.Clear();

            //_cancellationTokenSource?.Cancel();
            //_cancellationTokenSource?.Dispose();
            //_cancellationTokenSource = null;
        }

    }
}
