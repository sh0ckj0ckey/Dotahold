﻿using System;
using Dotahold.Data.DataShop;
using Dotahold.ViewModels;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Dotahold
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        internal MainViewModel MainViewModel { get; private set; }

        /// <summary>
        /// Initializes the singleton application object. This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            this.MainViewModel = new MainViewModel();
        }

        /// <inheritdoc/>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active.
            if (Window.Current.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame()
                {
                    RequestedTheme = this.MainViewModel.AppSettings.AppearanceIndex == 1 ? ElementTheme.Light : ElementTheme.Dark,
                };
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page, configuring
                    // the new page by passing required information as a navigation parameter.
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }

            _ = StorageFilesCourier.InitializeAsync();
            _ = ImageCourier.InitializeAsync();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            LogCourier.Log($"Failed to load page '{e.SourcePageType.FullName}'.", LogCourier.LogType.Error);
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when application execution is being suspended. Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            // TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            LogCourier.Log(e.Message, LogCourier.LogType.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                LogCourier.Log(exception.Message, LogCourier.LogType.Error);
            }
        }
    }
}
