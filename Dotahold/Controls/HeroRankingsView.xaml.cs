using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Controls
{
    public sealed partial class HeroRankingsView : UserControl
    {
        private static readonly Dictionary<int, List<HeroRankingModel>> _heroRankingModels = [];

        private readonly HeroModel _heroModel;

        private CancellationTokenSource? _cancellationTokenSource;

        public HeroRankingsView(HeroModel heroModel)
        {
            _heroModel = heroModel;

            this.InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await LoadHeroRankings(_heroModel.DotaHeroAttributes.id);
        }

        private void UserControl_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private async Task LoadHeroRankings(int heroId)
        {
            try
            {
                RankingsScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                LoadingProgressRing.IsActive = true;

                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource?.Token ?? CancellationToken.None;

                List<HeroRankingModel>? heroRankings = null;

                if (_heroRankingModels.TryGetValue(heroId, out var cachedRankingModels))
                {
                    await Task.Delay(600);
                    heroRankings = cachedRankingModels;
                }
                else
                {
                    heroRankings = [];
                    var dotaHeroRankingModels = await ApiCourier.GetHeroRankings(heroId, token);

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    if (dotaHeroRankingModels is not null)
                    {
                        for (int i = 0; i < dotaHeroRankingModels.Length; i++)
                        {
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }

                            heroRankings.Add(new HeroRankingModel(dotaHeroRankingModels[i], i + 1));
                        }

                        _heroRankingModels[heroId] = heroRankings;
                    }
                }

                RankingsItemsRepeater.ItemsSource = heroRankings;

                if (RankingsScrollViewer is not null)
                {
                    RankingsScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                if (LoadingProgressRing is not null)
                {
                    LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    LoadingProgressRing.IsActive = false;
                }

                foreach (var heroRanking in heroRankings)
                {
                    await heroRanking.AvatarImage.LoadImageAsync();

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex) { LogCourier.Log($"Loading hero rankings failed: {ex}", LogCourier.LogType.Error); }
            finally
            {
                if (RankingsScrollViewer is not null)
                {
                    RankingsScrollViewer.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                if (LoadingProgressRing is not null)
                {
                    LoadingProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    LoadingProgressRing.IsActive = false;
                }
            }
        }
    }
}
