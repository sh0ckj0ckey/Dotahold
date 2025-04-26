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
        private Dictionary<string, HeroModel> _heroModels { get; set; } = [];

        /// <summary>
        /// Heroes with Strength as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> StrHeroes { get; set; } = [];

        /// <summary>
        /// Heroes with Agility as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> AgiHeroes { get; set; } = [];

        /// <summary>
        /// Heroes with Intelligence as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> IntHeroes { get; set; } = [];

        /// <summary>
        /// Heroes with Universal as their Primary Attribute
        /// </summary>
        public ObservableCollection<HeroModel> UniHeroes { get; set; } = [];

        private bool _loading = false;

        /// <summary>
        /// Indicates whether the heroes list is being loaded
        /// </summary>
        public bool Loading
        {
            get => _loading;
            set => SetProperty(ref _loading, value);
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

                    _ = heroModel.HeroImage.LoadImageAsync();
                    _ = heroModel.HeroIcon.LoadImageAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.LogAsync($"Loading heroes failed: {ex.ToString()}", LogCourier.LogType.Error);
            }
            finally
            {
                this.Loading = false;
            }
        }
    }
}
