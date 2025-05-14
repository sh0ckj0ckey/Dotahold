using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Pages.Heroes
{
    public sealed partial class HeroRankingsView : UserControl
    {
        private static readonly Dictionary<int, List<HeroRankingModel>> _heroRankingModels = [];

        private readonly HeroModel _heroModel;

        private bool _unloaded = false;

        public HeroRankingsView(HeroModel heroModel)
        {
            _heroModel = heroModel;

            this.Loaded += (_, _) => _ = LoadHeroRankings(_heroModel.DotaHeroAttributes.id);
            this.Unloaded += (_, _) => _unloaded = true;

            this.InitializeComponent();
        }

        private async Task LoadHeroRankings(int heroId)
        {
            try
            {
                RankingsScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                LoadingProgressRing.IsActive = true;

                List<HeroRankingModel>? heroRankings = null;

                if (_heroRankingModels.TryGetValue(heroId, out var cachedRankingModels))
                {
                    await Task.Delay(600);
                    heroRankings = cachedRankingModels;
                }
                else
                {
                    heroRankings = [];
                    var dotaHeroRankingModels = await ApiCourier.GetHeroRankings(heroId);
                    if (dotaHeroRankingModels is not null)
                    {
                        for (int i = 0; i < dotaHeroRankingModels.Length; i++)
                        {
                            heroRankings.Add(new HeroRankingModel(dotaHeroRankingModels[i], i + 1));
                        }

                        _heroRankingModels[heroId] = heroRankings;
                    }
                }

                RankingsItemsRepeater.ItemsSource = heroRankings;

                RankingsScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LoadingProgressRing.IsActive = false;

                foreach (var heroRanking in heroRankings)
                {
                    await heroRanking.AvatarImage.LoadImageAsync();

                    if (_unloaded)
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Loading hero rankings failed: {ex}", LogCourier.LogType.Error);
            }
            finally
            {
                if (!_unloaded)
                {
                    RankingsScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    LoadingProgressRing.IsActive = false;
                }
            }
        }
    }
}
