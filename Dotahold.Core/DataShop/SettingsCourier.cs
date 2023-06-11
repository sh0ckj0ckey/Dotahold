using Dotahold.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Dotahold.Core.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的设置项
    /// </summary>
    public class SettingsCourier : ViewModelBase
    {
        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_STARTUPINDEX = "StartupPage";
        private const string SETTING_NAME_LANGUAGEINDEX = "Language";
        private const string SETTING_NAME_SEARCHMODE = "ItemsSearchFuzzy";
        private const string SETTING_NAME_STEAMID = "SteamID";

        private static Lazy<SettingsCourier> _lazyVM = new Lazy<SettingsCourier>(() => new SettingsCourier());
        public static SettingsCourier Instance => _lazyVM.Value;

        private ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        private SettingsCourier() { }

        // 设置的应用程序的主题 0-黑暗 1-明亮
        private int _iAppearanceIndex = -1;
        public int iAppearanceIndex
        {
            get
            {
                try
                {
                    if (_iAppearanceIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX] == null)
                        {
                            _iAppearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "0")
                        {
                            _iAppearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "1")
                        {
                            _iAppearanceIndex = 1;
                        }
                        else
                        {
                            _iAppearanceIndex = 0;
                        }
                    }
                }
                catch { }
                if (_iAppearanceIndex < 0) _iAppearanceIndex = 0;
                return _iAppearanceIndex < 0 ? 0 : _iAppearanceIndex;
            }
            set
            {
                Set("iAppearanceIndex", ref _iAppearanceIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_APPEARANCEINDEX] = _iAppearanceIndex;
            }
        }

        // 启动页面 0-Heroes 1-Items 2-Matches
        private int _iStartupPageIndex = -1;
        public int iStartupPageIndex
        {
            get
            {
                try
                {
                    if (_iStartupPageIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_STARTUPINDEX] == null)
                        {
                            _iStartupPageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_STARTUPINDEX]?.ToString() == "0")
                        {
                            _iStartupPageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_STARTUPINDEX]?.ToString() == "1")
                        {
                            _iStartupPageIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_STARTUPINDEX]?.ToString() == "2")
                        {
                            _iStartupPageIndex = 2;
                        }
                        else
                        {
                            _iStartupPageIndex = 0;
                        }
                    }
                }
                catch { }
                if (_iStartupPageIndex < 0) _iStartupPageIndex = 0;
                return _iStartupPageIndex < 0 ? 0 : _iStartupPageIndex;
            }
            set
            {
                Set("iStartupPageIndex", ref _iStartupPageIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_STARTUPINDEX] = _iStartupPageIndex;
            }
        }

        // 语言 0-English 1-Chinese 2-Russian
        private int _iLanguageIndex = -1;
        public int iLanguageIndex
        {
            get
            {
                try
                {
                    if (_iLanguageIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX] == null)
                        {
                            _iLanguageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX].ToString() == "0")
                        {
                            _iLanguageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX].ToString() == "1")
                        {
                            _iLanguageIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX].ToString() == "2")
                        {
                            _iLanguageIndex = 2;
                        }
                        else
                        {
                            _iLanguageIndex = 0;
                        }
                    }
                }
                catch { }
                if (_iLanguageIndex < 0) _iLanguageIndex = 0;
                return _iLanguageIndex < 0 ? 0 : _iLanguageIndex;
            }
            set
            {
                Set("iLanguageIndex", ref _iLanguageIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_LANGUAGEINDEX] = _iLanguageIndex;
            }
        }

        // 物品页是否开启模糊搜索
        private bool? _bItemsSearchFuzzy = null;
        public bool bItemsSearchFuzzy
        {
            get
            {
                try
                {
                    if (_bItemsSearchFuzzy == null)
                    {
                        if (_localSettings.Values[SETTING_NAME_SEARCHMODE] == null)
                        {
                            _bItemsSearchFuzzy = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHMODE].ToString() == "True")
                        {
                            _bItemsSearchFuzzy = true;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHMODE].ToString() == "False")
                        {
                            _bItemsSearchFuzzy = false;
                        }
                        else
                        {
                            _bItemsSearchFuzzy = false;
                        }
                    }
                }
                catch { }
                if (_bItemsSearchFuzzy == null) _bItemsSearchFuzzy = false;
                return _bItemsSearchFuzzy ?? false;
            }
            set
            {
                Set("bItemsSearchFuzzy", ref _bItemsSearchFuzzy, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_SEARCHMODE] = _bItemsSearchFuzzy;
            }
        }

        //// 直播间亮度(0.0~100.0)
        //private double _dBrightness = -1;
        //public double dBrightness
        //{
        //    get
        //    {
        //        // 读取直播间亮度
        //        try
        //        {
        //            if (_dBrightness < 0)
        //            {
        //                if (_localSettings.Values[SETTING_NAME_BRIGHTNESS] == null)
        //                {
        //                    _dBrightness = 100;
        //                }
        //                else
        //                {
        //                    string volumeStr = _localSettings.Values[SETTING_NAME_BRIGHTNESS].ToString();
        //                    if (double.TryParse(volumeStr, out double volume))
        //                    {
        //                        _dBrightness = volume;
        //                    }
        //                }
        //            }
        //        }
        //        catch { }
        //        if (_dBrightness < 0) _dBrightness = 100;
        //        return _dBrightness < 0 ? 100 : _dBrightness;
        //    }
        //    set
        //    {
        //        Set("dBrightness", ref _dBrightness, value);
        //        ApplicationData.Current.LocalSettings.Values[SETTING_NAME_BRIGHTNESS] = _dBrightness;
        //    }
        //}

        // SteamID
        private string _sSteamID = null;
        public string sSteamID
        {
            get
            {
                try
                {
                    if (_sSteamID == null)
                    {
                        if (_localSettings.Values[SETTING_NAME_STEAMID] == null)
                        {
                            _sSteamID = "";
                        }
                        else
                        {
                            string steamId = _localSettings.Values[SETTING_NAME_STEAMID].ToString();
                            _sSteamID = steamId;
                        }
                    }
                }
                catch { }
                if (_sSteamID == null) _sSteamID = "";
                return _sSteamID == null ? "" : _sSteamID;
            }
            set
            {
                Set("sSteamID", ref _sSteamID, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_STEAMID] = _sSteamID;
            }
        }
    }
}
