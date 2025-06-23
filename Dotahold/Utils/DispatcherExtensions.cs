using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Dotahold.Utils
{
    internal static class DispatcherExtensions
    {
        public static async Task CallOnUiThreadAsync(CoreDispatcher dispatcher, DispatchedHandler handler, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal) =>
            await dispatcher.RunAsync(priority, handler);

        public static async Task CallOnMainViewUiThreadAsync(DispatchedHandler handler, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal) =>
            await CallOnUiThreadAsync(CoreApplication.MainView.CoreWindow.Dispatcher, handler, priority);
    }
}
