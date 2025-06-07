using System;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Controls
{
    public sealed partial class WinLoseControl : UserControl
    {
        public int WinValue
        {
            get => (int)GetValue(WinValueProperty);
            set => SetValue(WinValueProperty, value);
        }

        public static readonly DependencyProperty WinValueProperty =
            DependencyProperty.Register(nameof(WinValue), typeof(int), typeof(WinLoseControl), new PropertyMetadata(0, OnWinOrLoseChanged));

        public int LoseValue
        {
            get => (int)GetValue(LoseValueProperty);
            set => SetValue(LoseValueProperty, value);
        }

        public static readonly DependencyProperty LoseValueProperty =
            DependencyProperty.Register(nameof(LoseValue), typeof(int), typeof(WinLoseControl), new PropertyMetadata(0, OnWinOrLoseChanged));

        public string WinRate
        {
            get => (string)GetValue(WinRateProperty);
            set => SetValue(WinRateProperty, value);
        }

        public static readonly DependencyProperty WinRateProperty =
            DependencyProperty.Register(nameof(WinRate), typeof(string), typeof(WinLoseControl), new PropertyMetadata(string.Empty));

        private static void OnWinOrLoseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WinLoseControl control)
            {
                control.UpdateWinLoseBar();
            }
        }

        private void UpdateWinLoseBar()
        {
            try
            {
                double rate = 0.5;

                if (this.LoseValue <= 0)
                {
                    rate = 1;
                }
                else if (this.WinValue <= 0)
                {
                    rate = 0;
                }
                else
                {
                    rate = (double)this.WinValue / (this.WinValue + this.LoseValue);
                    rate = Math.Floor(100 * rate) / 100;
                }

                AllLoseBorder.Visibility = Visibility.Collapsed;
                AllWinBorder.Visibility = Visibility.Collapsed;
                WinLoseBarGrid.Visibility = Visibility.Collapsed;

                if (rate <= 0)
                {
                    AllLoseBorder.Visibility = Visibility.Visible;
                }
                else if (rate >= 1)
                {
                    AllWinBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    WinLoseBarGrid.Visibility = Visibility.Visible;
                    rate *= 100;
                    WinColumn.Width = new GridLength(rate, GridUnitType.Star);
                    LoseColumn.Width = new GridLength(100 - rate, GridUnitType.Star);
                }
            }
            catch (Exception ex) { LogCourier.Log($"UpdateWinLoseBar({this.WinValue},{this.LoseValue}) error: {ex.Message}", LogCourier.LogType.Error); }
        }

        public WinLoseControl()
        {
            this.InitializeComponent();
        }

        private void WinLoseGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ShowWinLoseTextBlockStoryboard.Begin();
        }

        private void WinLoseGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ShowWinRateTextBlockStoryboard.Begin();
        }
    }
}
