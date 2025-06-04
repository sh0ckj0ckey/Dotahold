using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class HeroesViewModel : ObservableObject
    {
        /// <summary>
        /// A semaphore used to limit concurrent access to image loading operations.
        /// </summary>
        private static readonly SemaphoreSlim _imageLoadSemaphore = new(1);

        /// <summary>
        /// Task to load heroes, used to prevent multiple simultaneous loads
        /// </summary>
        private Task? _loadHeroesTask = null;

        /// <summary>
        /// HeroId to HeroModel
        /// </summary>
        private readonly Dictionary<string, HeroModel> _heroModels = [];

        /// <summary>
        /// Language to HeroId to HeroDataModel
        /// </summary>
        private readonly Dictionary<int, Dictionary<int, HeroDataModel>> _heroDataModels = [];

        /// <summary>
        /// Heroes with Strength as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> StrHeroes { get; } = [];

        /// <summary>
        /// Heroes with Agility as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> AgiHeroes { get; } = [];

        /// <summary>
        /// Heroes with Intelligence as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> IntHeroes { get; } = [];

        /// <summary>
        /// Heroes with Universal as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> UniHeroes { get; } = [];

        private bool _loading = false;

        private bool _loadingHeroData = false;

        private HeroModel? _pickedHero = null;

        private HeroDataModel? _pickedHeroData = null;

        /// <summary>
        /// Indicates whether the heroes list is being loaded
        /// </summary>
        public bool Loading
        {
            get => _loading;
            set => SetProperty(ref _loading, value);
        }

        /// <summary>
        /// Indicates whether the hero data is being loaded
        /// </summary>
        public bool LoadingHeroData
        {
            get => _loadingHeroData;
            set => SetProperty(ref _loadingHeroData, value);
        }

        /// <summary>
        /// The currently picked hero
        /// </summary>
        public HeroModel? PickedHero
        {
            get => _pickedHero;
            set => SetProperty(ref _pickedHero, value);
        }

        /// <summary>
        /// The hero data of the currently picked hero
        /// </summary>
        public HeroDataModel? PickedHeroData
        {
            get => _pickedHeroData;
            set => SetProperty(ref _pickedHeroData, value);
        }

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

        public async Task LoadHeroes()
        {
            if (_loadHeroesTask is not null)
            {
                await _loadHeroesTask;
                return;
            }

            _loadHeroesTask = LoadHeroesInternal();

            try
            {
                await _loadHeroesTask;
            }
            finally
            {
                _loadHeroesTask = null;
            }
        }

        private async Task LoadHeroesInternal()
        {
            try
            {
                if (this.Loading || _heroModels.Count > 0)
                {
                    return;
                }

                this.Loading = true;

                _heroModels.Clear();
                this.StrHeroes.Clear();
                this.AgiHeroes.Clear();
                this.IntHeroes.Clear();
                this.UniHeroes.Clear();

                Dictionary<string, Data.Models.DotaHeroModel> heroesConstant = await ConstantsCourier.GetHeroesConstant();

                foreach (var hero in heroesConstant.Values)
                {
                    var heroModel = new HeroModel(hero);

                    string primary = heroModel.DotaHeroAttributes.primary_attr?.ToLower() ?? "";
                    if (primary.Contains("str"))
                    {
                        this.StrHeroes.Add(heroModel);
                    }
                    else if (primary.Contains("agi"))
                    {
                        this.AgiHeroes.Add(heroModel);
                    }
                    else if (primary.Contains("int"))
                    {
                        this.IntHeroes.Add(heroModel);
                    }
                    else if (primary.Contains("all"))
                    {
                        this.UniHeroes.Add(heroModel);
                    }
                    else
                    {
                        this.UniHeroes.Add(heroModel);
                    }

                    _heroModels[heroModel.DotaHeroAttributes.id.ToString()] = heroModel;
                }

                this.Loading = false;

                foreach (var hero in this.StrHeroes)
                {
                    _ = SafeLoadImageAsync(() => hero.HeroImage.LoadImageAsync());
                    _ = SafeLoadImageAsync(() => hero.HeroIcon.LoadImageAsync());
                }
                foreach (var hero in this.AgiHeroes)
                {
                    _ = SafeLoadImageAsync(() => hero.HeroImage.LoadImageAsync());
                    _ = SafeLoadImageAsync(() => hero.HeroIcon.LoadImageAsync());
                }
                foreach (var hero in this.IntHeroes)
                {
                    _ = SafeLoadImageAsync(() => hero.HeroImage.LoadImageAsync());
                    _ = SafeLoadImageAsync(() => hero.HeroIcon.LoadImageAsync());
                }
                foreach (var hero in this.UniHeroes)
                {
                    _ = SafeLoadImageAsync(() => hero.HeroImage.LoadImageAsync());
                    _ = SafeLoadImageAsync(() => hero.HeroIcon.LoadImageAsync());
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Loading heroes failed: {ex}", LogCourier.LogType.Error);
            }
            finally
            {
                this.Loading = false;
            }
        }

        public async Task PickHero(HeroModel heroModel, int languageIndex)
        {
            try
            {
                this.LoadingHeroData = true;

                this.PickedHero = heroModel;

                string language = languageIndex switch
                {
                    0 => "english",
                    1 => "schinese",
                    2 => "russian",
                    _ => "english"
                };

                if (_heroDataModels.TryGetValue(languageIndex, out var dataModels)
                    && dataModels.TryGetValue(heroModel.DotaHeroAttributes.id, out var dataModel))
                {
                    await Task.Delay(600);
                    this.PickedHeroData = dataModel;
                }
                else
                {
                    var dotaHeroDataModel = await ApiCourier.GetHeroData(heroModel.DotaHeroAttributes.id, language, CancellationToken.None);
                    if (dotaHeroDataModel is not null)
                    {
                        if (!_heroDataModels.TryGetValue(languageIndex, out _))
                        {
                            _heroDataModels[languageIndex] = [];
                        }

                        _heroDataModels[languageIndex][heroModel.DotaHeroAttributes.id] = new HeroDataModel(dotaHeroDataModel);
                        this.PickedHeroData = _heroDataModels[languageIndex][heroModel.DotaHeroAttributes.id];
                    }
                }

                this.LoadingHeroData = false;

                if (this.PickedHeroData is not null)
                {
                    foreach (var facet in this.PickedHeroData.Facets)
                    {
                        await facet.IconImage.LoadImageAsync();
                    }

                    foreach (var ability in this.PickedHeroData.Abilities)
                    {
                        if (!ability.IsInnateAbility)
                        {
                            await ability.IconImage.LoadImageAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Getting hero data failed: {ex}", LogCourier.LogType.Error);
            }
            finally
            {
                this.LoadingHeroData = false;
            }
        }
    }
}
