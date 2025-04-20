using System;
using Dotahold.ViewModels;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.UI
{
    public sealed partial class HeroHistoryDialogContent : Page
    {
        DotaHeroesViewModel ViewModel = null;
        public HeroHistoryDialogContent()
        {
            this.InitializeComponent();

            try
            {
                ViewModel = DotaHeroesViewModel.Instance;
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }
}
