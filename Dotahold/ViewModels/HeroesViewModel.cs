using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Dotahold.Data.DataShop;
using Dotahold.Models;

namespace Dotahold.ViewModels
{
    internal partial class HeroesViewModel : ObservableObject
    {
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

        public async Task LoadHeroes()
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
                    await hero.HeroImage.LoadImageAsync();
                    await hero.HeroIcon.LoadImageAsync();
                }
                foreach (var hero in this.AgiHeroes)
                {
                    await hero.HeroImage.LoadImageAsync();
                    await hero.HeroIcon.LoadImageAsync();
                }
                foreach (var hero in this.IntHeroes)
                {
                    await hero.HeroImage.LoadImageAsync();
                    await hero.HeroIcon.LoadImageAsync();
                }
                foreach (var hero in this.UniHeroes)
                {
                    await hero.HeroImage.LoadImageAsync();
                    await hero.HeroIcon.LoadImageAsync();
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
                    return;
                }

                var dotaHeroDataModel = await ApiCourier.GetHeroData(heroModel.DotaHeroAttributes.id, language);
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
