using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Helpers
{
    public class NetworkCheckHelper
    {
        public static bool NetworkAvailable = false;

        public static bool CheckNetwork()
        {
            try
            {
                NetworkAvailable = NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable;
            }
            catch
            {
                NetworkAvailable = false;
            }
            return NetworkAvailable;
        }
    }
}
