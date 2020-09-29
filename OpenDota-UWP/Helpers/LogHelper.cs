using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System;

namespace OpenDota_UWP.Helpers
{
    public static class LogHelper
    {
        private static MetroLog.ILogger logger = null;

        public static void Log(string log)
        {
            try
            {
                if (logger == null)
                {
                    logger = MetroLog.LogManagerFactory.CreateLogManager().GetLogger("OpenDotaLog");
                }
                logger.Error(log);
            }
            catch { }
        }

        public static void ShowLogFolder()
        {
            _ = Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }
    }
}
