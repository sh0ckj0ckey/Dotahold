﻿using System;
using Windows.ApplicationModel;

namespace Dotahold.Utils
{
    internal static class AppVersionUtil
    {
        /// <summary>
        /// 获取app版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersion()
        {
            try
            {
                Package package = Package.Current;
                PackageId packageId = package.Id;
                PackageVersion version = packageId.Version;
                return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            }
            catch (Exception) { }
            return "";
        }
    }
}
