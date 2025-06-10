using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class MatchesViewModel(HeroesViewModel heroesViewModel, ItemsViewModel itemsViewModel) : ObservableObject
    {
        private CancellationTokenSource? _cancellationTokenSource;

        /// <summary>
        /// A semaphore used to limit concurrent access to image loading operations.
        /// </summary>
        private static readonly SemaphoreSlim _imageLoadSemaphore = new(1);

        /// <summary>
        /// Task to load abilities, used to prevent multiple simultaneous loads
        /// </summary>
        private Task? _loadAbilitiesTask = null;

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

        private static async Task SafeLoadImageAsync(Func<Task> loadImageFunc)
        {
            await _imageLoadSemaphore.WaitAsync();
            try
            {
                await loadImageFunc();
            }
            finally
            {
                _imageLoadSemaphore.Release();
            }
        }

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
    }
}
