using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Controls
{
    public sealed partial class MatchSearchView : UserControl
    {
        private readonly Action _hideDialogContent;

        public MatchSearchView(Action hideContentDialog)
        {
            _hideDialogContent = hideContentDialog;
            this.InitializeComponent();
        }

        public string GetMatchId()
        {
            return string.Empty;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _hideDialogContent.Invoke();
        }
    }
}
