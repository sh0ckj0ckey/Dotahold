using Dotahold.Core.DataShop;
using Dotahold.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Dotahold.ViewModels
{
    public class DotaViewModel : ViewModelBase
    {
        private static Lazy<DotaViewModel> _lazyVM = new Lazy<DotaViewModel>(() => new DotaViewModel());
        public static DotaViewModel Instance => _lazyVM.Value;

        // Setting
        public SettingsCourier AppSettings { get; set; } = SettingsCourier.Instance;

        // 切换主题时，更新标题栏样式
        public Action ActChangeTitleBarTheme { get; set; } = null;

        // 当前选中的Tab: 0-Heroes 1-Items 2-Matches 3-Settings
        private int _iSideMenuTabIndex = 0;
        public int iSideMenuTabIndex
        {
            get { return _iSideMenuTabIndex; }
            set { Set("iSideMenuTabIndex", ref _iSideMenuTabIndex, value); }
        }

        // 图片缓存大小
        private string _sImageCacheSize = string.Empty;
        public string sImageCacheSize
        {
            get { return _sImageCacheSize; }
            set { Set("sImageCacheSize", ref _sImageCacheSize, value); }
        }

        // 是否正在清理图片缓存
        private bool _bCleaningImageCache = false;
        public bool bCleaningImageCache
        {
            get { return _bCleaningImageCache; }
            set { Set("bCleaningImageCache", ref _bCleaningImageCache, value); }
        }

        // 是否显示开发者工具
        private bool _bShowDevTools = false;
        public bool bShowDevTools
        {
            get { return _bShowDevTools; }
            set { Set("bShowDevTools", ref _bShowDevTools, value); }
        }

        private DotaViewModel() { }

        /// <summary>
        /// 获取图片缓存的大小
        /// </summary>
        public async void GetImageCacheSize()
        {
            try
            {
                async Task<long> funcGetCacheSize()
                {
                    long sizeTmp = await ImageCourier.GetCacheSizeAsync();
                    return sizeTmp;
                }

                long size = await Task.Run(funcGetCacheSize);

                if (size > 0)
                {
                    sImageCacheSize = ByteConvert2GBMBKB(size);
                }
                else
                {
                    sImageCacheSize = "0B";
                }
            }
            catch { sImageCacheSize = string.Empty; }
        }

        /// <summary>
        /// 清理图片缓存
        /// </summary>
        public async void ClearImageCache()
        {
            try
            {
                bCleaningImageCache = true;
                bool result = await ImageCourier.ClearCacheAsync();

                if (result) GetImageCacheSize();
            }
            catch { }
            finally { bCleaningImageCache = false; }
        }

        private string ByteConvert2GBMBKB(long size)
        {
            try
            {
                if (size / 1073741824 >= 1)//如果当前Byte的值大于等于1GB
                    return (Math.Round(size / (float)1073741824, 2)).ToString() + "GB";// 将其转换成 GB
                else if (size / 1048576 >= 1)//如果当前Byte的值大于等于1MB
                    return (Math.Round(size / (float)1048576, 2)).ToString() + "MB";// 将其转换成 MB
                else if (size / 1024 >= 1)//如果当前Byte的值大于等于1KB
                    return (Math.Round(size / (float)1024, 2)).ToString() + "KB";// 将其转换成 KB
                else
                    return size.ToString() + "B"; // 显示 Byte 值
            }
            catch { }
            return size.ToString() + "B";
        }
    }
}
