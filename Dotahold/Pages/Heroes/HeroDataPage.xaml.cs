using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dotahold.Controls;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Dotahold.ViewModels;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Dotahold.Pages.Heroes
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HeroDataPage : Page
    {
        private readonly Visual _visual;

        private readonly MainViewModel _viewModel;

        private HeroModel? _heroModel;

        private readonly List<HeroFacetCard> _facetCards = [];

        public HeroDataPage()
        {
            _viewModel = App.Current.MainViewModel;

            this.InitializeComponent();

            _visual = ElementCompositionPreview.GetElementVisual(this);

            this.Loaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated += CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;

                DataAttributesScrollViewer.Width = RootGrid.ActualWidth;
                FacetsGrid.Width = RootGrid.ActualWidth - 128;
            };

            this.Unloaded += (_, _) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -= CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                SystemNavigationManager.GetForCurrentView().BackRequested -= System_BackRequested;
            };
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                ConnectedAnimation? animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("Hero");
                animation?.TryStart(HeroImageBorder);

                _heroModel = e.Parameter as HeroModel;

                if (_heroModel is not null)
                {
                    await _viewModel.HeroesViewModel.PickHero(_heroModel, _viewModel.AppSettings.LanguageIndex);

                    // 
                    // TODO: 不能这样做，PickHero会等待图标下载，耗时较久
                    // 而且此处似乎控件都还没有创建，不应当在这里修改控件
                    // 只是记录一下思路，以及如下为封装成为一个通用控件
                    // 
                    // using Microsoft.UI.Xaml.Controls;
                    // using Windows.UI.Xaml;
                    // using Windows.UI.Xaml.Controls;
                    // using Windows.UI.Xaml.Media;
                    // using System;

                    // public class AutoWrapGridPanel : Panel
                    // {
                    //     public double MinItemWidth
                    //     {
                    //         get => (double)GetValue(MinItemWidthProperty);
                    //         set => SetValue(MinItemWidthProperty, value);
                    //     }
                    //     public static readonly DependencyProperty MinItemWidthProperty =
                    //         DependencyProperty.Register(nameof(MinItemWidth), typeof(double), typeof(AutoWrapGridPanel), new PropertyMetadata(296.0, LayoutPropertyChanged));

                    //     private static void LayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
                    //     {
                    //         if (d is AutoWrapGridPanel panel)
                    //             panel.InvalidateMeasure();
                    //     }

                    //     protected override Size MeasureOverride(Size availableSize)
                    //     {
                    //         double width = availableSize.Width;
                    //         int count = Children.Count;
                    //         if (count == 0 || double.IsInfinity(width) || width == 0)
                    //             return new Size(width, 0);

                    //         double minWidth = MinItemWidth;
                    //         int columns = Math.Max(1, Math.Min(count, (int)(width / minWidth)));
                    //         double itemWidth = width / columns;

                    //         double x = 0, y = 0, rowHeight = 0;
                    //         int col = 0;
                    //         foreach (UIElement child in Children)
                    //         {
                    //             child.Measure(new Size(itemWidth, double.PositiveInfinity));
                    //             rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
                    //             col++;
                    //             if (col == columns)
                    //             {
                    //                 y += rowHeight;
                    //                 rowHeight = 0;
                    //                 col = 0;
                    //             }
                    //         }
                    //         if (col != 0)
                    //             y += rowHeight;

                    //         return new Size(width, y);
                    //     }

                    //     protected override Size ArrangeOverride(Size finalSize)
                    //     {
                    //         double width = finalSize.Width;
                    //         int count = Children.Count;
                    //         if (count == 0 || width == 0)
                    //             return finalSize;

                    //         double minWidth = MinItemWidth;
                    //         int columns = Math.Max(1, Math.Min(count, (int)(width / minWidth)));
                    //         double itemWidth = width / columns;

                    //         double x = 0, y = 0, rowHeight = 0;
                    //         int col = 0;
                    //         foreach (UIElement child in Children)
                    //         {
                    //             child.Measure(new Size(itemWidth, double.PositiveInfinity));
                    //             var childHeight = child.DesiredSize.Height;
                    //             child.Arrange(new Rect(x, y, itemWidth, childHeight));
                    //             rowHeight = Math.Max(rowHeight, childHeight);
                    //             x += itemWidth;
                    //             col++;
                    //             if (col == columns)
                    //             {
                    //                 x = 0;
                    //                 y += rowHeight;
                    //                 rowHeight = 0;
                    //                 col = 0;
                    //             }
                    //         }
                    //         return finalSize;
                    //     }
                    // }
                    // 
                    // 
                    // 使用
                    //< ItemsControl ItemsSource = "{Binding Facets}" >
                    //    < ItemsControl.ItemsPanel >
                    //        < ItemsPanelTemplate >
                    //            < local:AutoWrapGridPanel MinItemWidth = "296" />
                    //        </ ItemsPanelTemplate >
                    //    </ ItemsControl.ItemsPanel >
                    //    < ItemsControl.ItemTemplate >
                    //        < DataTemplate >
                    //            < controls:HeroFacetCard DataContext = "{Binding}" />
                    //        </ DataTemplate >
                    //    </ ItemsControl.ItemTemplate >
                    //</ ItemsControl >

                    _facetCards.Clear();
                    FacetsGrid.Children.Clear();

                    if (_viewModel.HeroesViewModel.PickedHeroData?.Facets is not null)
                    {
                        foreach (var facet in _viewModel.HeroesViewModel.PickedHeroData?.Facets)
                        {
                            var facetCard = new HeroFacetCard(facet);
                            _facetCards.Add(facetCard);
                            FacetsGrid.Children.Add(facetCard);
                        }
                    }

                    DataAttributesScrollViewer.Width = RootGrid.ActualWidth;
                    FacetsGrid.Width = RootGrid.ActualWidth - 128;
                    UpdateFacetsLayout();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                ConnectedAnimationService.GetForCurrentView().DefaultDuration = TimeSpan.FromSeconds(0.3);
                ConnectedAnimationService.GetForCurrentView().DefaultEasingFunction = CompositionEasingFunction.CreateCubicBezierEasingFunction(_visual.Compositor, new(0.41f, 0.52f), new(0.0f, 0.94f));
                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("HeroBack", HeroImageBorder);
                animation.Configuration = new BasicConnectedAnimationConfiguration();
            }
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataAttributesScrollViewer.Width = RootGrid.ActualWidth;
            FacetsGrid.Width = RootGrid.ActualWidth - 128;
            UpdateFacetsLayout();
        }

        private void UpdateFacetsLayout()
        {
            FacetsGrid.RowDefinitions.Clear();
            FacetsGrid.ColumnDefinitions.Clear();

            double gridWidth = FacetsGrid.ActualWidth > 0 ? FacetsGrid.ActualWidth : FacetsGrid.Width;
            int minCardWidth = 296;
            int columnCount = Math.Max(1, Math.Min(_facetCards.Count, (int)(gridWidth / minCardWidth)));
            int rowCount = (_facetCards.Count + columnCount - 1) / columnCount;

            for (int i = 0; i < columnCount; i++)
            {
                FacetsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int i = 0; i < rowCount; i++)
            {
                FacetsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < _facetCards.Count; i++)
            {
                int row = i / columnCount;
                int col = i % columnCount;
                Grid.SetRow(_facetCards[i], row);
                Grid.SetColumn(_facetCards[i], col);
            }
        }

        /// <summary>
        /// 查看英雄背景故事
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.HeroesViewModel.PickedHero is not null)
                {
                    await (new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        RequestedTheme = this.ActualTheme,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        IsPrimaryButtonEnabled = false,
                        IsSecondaryButtonEnabled = false,
                        CloseButtonText = "Close",
                        Content = new HeroHistoryView(_viewModel.HeroesViewModel.PickedHero, _viewModel.HeroesViewModel.PickedHeroData?.DotaHeroData?.bio_loc ?? "Failed to fetch hero's history."),
                    }).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Show hero history failed: {ex.Message}", LogCourier.LogType.Error);
            }
        }

        /// <summary>
        /// 查看英雄排名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RankingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_viewModel.HeroesViewModel.PickedHero is not null)
                {
                    await (new ContentDialog
                    {
                        XamlRoot = this.XamlRoot,
                        RequestedTheme = this.ActualTheme,
                        Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        IsPrimaryButtonEnabled = false,
                        IsSecondaryButtonEnabled = false,
                        CloseButtonText = "Close",
                        Content = new HeroRankingsView(_viewModel.HeroesViewModel.PickedHero),
                    }).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Show hero rankings failed: {ex.Message}", LogCourier.LogType.Error);
            }
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton btn)
            {
                string languageName = btn.Content.ToString() ?? "English";

                int index = languageName switch
                {
                    "English" => 0,
                    "Chinese" => 1,
                    "Russian" => 2,
                    _ => 0
                };

                if (_viewModel.AppSettings.LanguageIndex != index)
                {
                    LanguageFlyout?.Hide();

                    _viewModel.AppSettings.LanguageIndex = index;

                    if (_heroModel is not null)
                    {
                        _ = _viewModel.HeroesViewModel.PickHero(_heroModel, _viewModel.AppSettings.LanguageIndex);
                    }
                }
            }
        }

        /// <summary>
        /// 查看天赋树
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TalentsButton_Click(object sender, RoutedEventArgs e)
        {
            HeroTalentsTeachingTip.IsOpen = !HeroTalentsTeachingTip.IsOpen;
        }

        /// <summary>
        /// 查看先天技能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InnateButton_Click(object sender, RoutedEventArgs e)
        {
            HeroInnateTeachingTip.IsOpen = !HeroInnateTeachingTip.IsOpen;
        }

        #region GoBack

        private bool TryGoBack()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                && e.VirtualKey == VirtualKey.Left
                && e.KeyStatus.IsMenuKeyDown == true
                && !e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void System_BackRequested(object? sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
            {
                e.Handled = TryGoBack();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            TryGoBack();
        }

        #endregion

    }
}
