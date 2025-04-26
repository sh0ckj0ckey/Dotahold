using System;
using Dotahold.Data.DataShop;
using Dotahold.Pages;
using Dotahold.Pages.Heroes;
using Dotahold.Pages.Items;
using Dotahold.Pages.Matches;
using Dotahold.ViewModels;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Dotahold
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a <see cref="Frame">.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly MainViewModel _viewModel;

        public MainPage()
        {
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            _viewModel = new MainViewModel();

            _viewModel.AppSettings.AppearanceSettingChanged += (_, _) =>
            {
                UpdateAppTheme();
            };

            this.Loaded += (_, _) =>
            {
                Window.Current.SetTitleBar(AppTitleBarGrid);
                MainFrameNavigateToPage(_viewModel.AppSettings.StartupPageIndex);
            };

            UpdateAppTheme();

            InitializeComponent();

            _ = _viewModel.HeroesViewModel.LoadHeroes();
            _ = _viewModel.ItemsViewModel.LoadItems();
        }

        private void HeroesTabButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainFrameNavigateToPage(0);
        }

        private void ItemsTabButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainFrameNavigateToPage(1);
        }

        private void MatchesTabButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainFrameNavigateToPage(2);
        }

        private void SettingsTabButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainFrameNavigateToPage(3);
        }

        private void MainFrameNavigateToPage(int pageIndex)
        {
            _viewModel.TabIndex = pageIndex;

            switch (_viewModel.TabIndex)
            {
                case 0:
                    if (!Type.Equals(MainFrame.CurrentSourcePageType, typeof(HeroesPage)))
                    {
                        MainFrame.Navigate(typeof(HeroesPage), _viewModel);
                    }
                    break;
                case 1:
                    if (!Type.Equals(MainFrame.CurrentSourcePageType, typeof(ItemsPage)))
                    {
                        MainFrame.Navigate(typeof(ItemsPage), _viewModel);
                    }
                    break;
                case 2:
                    if (!Type.Equals(MainFrame.CurrentSourcePageType, typeof(OverviewPage)))
                    {
                        MainFrame.Navigate(typeof(OverviewPage), _viewModel);
                    }
                    break;
                case 3:
                    if (!Type.Equals(MainFrame.CurrentSourcePageType, typeof(SettingsPage)))
                    {
                        MainFrame.Navigate(typeof(SettingsPage), _viewModel);
                    }
                    break;
            }
        }

        private void BackTabButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void UpdateAppTheme()
        {
            try
            {
                bool isLight = _viewModel.AppSettings.AppearanceIndex == 1;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                // Set active window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.ForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.BackgroundColor = Colors.Transparent;
                titleBar.ButtonForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonHoverForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonHoverBackgroundColor = isLight ? Windows.UI.Color.FromArgb(10, 0, 0, 0) : Windows.UI.Color.FromArgb(16, 255, 255, 255);
                titleBar.ButtonPressedForegroundColor = isLight ? Colors.Black : Colors.White;
                titleBar.ButtonPressedBackgroundColor = isLight ? Windows.UI.Color.FromArgb(08, 0, 0, 0) : Windows.UI.Color.FromArgb(10, 255, 255, 255);

                // Set inactive window colors
                // Note: No effect when app is running on Windows 10 since color customization is not supported.
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.InactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                if (Window.Current.Content is FrameworkElement rootElement)
                {
                    if (!isLight)
                    {
                        rootElement.RequestedTheme = ElementTheme.Dark;
                    }
                    else
                    {
                        rootElement.RequestedTheme = ElementTheme.Light;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error);
            }
        }
    }
}
