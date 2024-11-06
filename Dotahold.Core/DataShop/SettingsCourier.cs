using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Dotahold.Core.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的设置项
    /// </summary>
    public class SettingsCourier : ObservableObject
    {
        private const string SETTING_NAME_APPEARANCEINDEX = "AppearanceIndex";
        private const string SETTING_NAME_STARTUPINDEX = "StartupPage";
        private const string SETTING_NAME_LANGUAGEINDEX = "Language";
        private const string SETTING_NAME_SEARCHMODE = "ItemsSearchFuzzy";
        private const string SETTING_NAME_STEAMID = "SteamID";

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public Action<int> OnAppearanceSettingChanged { get; set; } = null;

        private int _appearanceIndex = -1;
        private int _startupPageIndex = -1;
        private int _languageIndex = -1;
        private bool? _itemsSearchFuzzy = null;
        private string _steamID = null;

        /// <summary>
        /// 应用程序的主题 0-黑暗 1-明亮
        /// </summary>
        public int AppearanceIndex
        {
            get
            {
                try
                {
                    if (_appearanceIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX] == null)
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "0")
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_APPEARANCEINDEX]?.ToString() == "1")
                        {
                            _appearanceIndex = 1;
                        }
                        else
                        {
                            _appearanceIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                if (_appearanceIndex < 0) _appearanceIndex = 0;
                return _appearanceIndex < 0 ? 0 : _appearanceIndex;
            }
            set
            {
                SetProperty(ref _appearanceIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_APPEARANCEINDEX] = _appearanceIndex;
                OnAppearanceSettingChanged?.Invoke(_appearanceIndex);
            }
        }

        /// <summary>
        /// 启动页面 0-Heroes 1-Items 2-Matches
        /// </summary>
        public int StartupPageIndex
        {
            get
            {
                try
                {
                    if (_startupPageIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_STARTUPINDEX] == null)
                        {
                            _startupPageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_STARTUPINDEX]?.ToString() == "0")
                        {
                            _startupPageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_STARTUPINDEX]?.ToString() == "1")
                        {
                            _startupPageIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_STARTUPINDEX]?.ToString() == "2")
                        {
                            _startupPageIndex = 2;
                        }
                        else
                        {
                            _startupPageIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                if (_startupPageIndex < 0) _startupPageIndex = 0;
                return _startupPageIndex < 0 ? 0 : _startupPageIndex;
            }
            set
            {
                SetProperty(ref _startupPageIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_STARTUPINDEX] = _startupPageIndex;
            }
        }

        /// <summary>
        /// 语言 0-English 1-Chinese 2-Russian
        /// </summary>
        public int LanguageIndex
        {
            get
            {
                try
                {
                    if (_languageIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX] == null)
                        {
                            _languageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX].ToString() == "0")
                        {
                            _languageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX].ToString() == "1")
                        {
                            _languageIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_NAME_LANGUAGEINDEX].ToString() == "2")
                        {
                            _languageIndex = 2;
                        }
                        else
                        {
                            _languageIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                if (_languageIndex < 0) _languageIndex = 0;
                return _languageIndex < 0 ? 0 : _languageIndex;
            }
            set
            {
                SetProperty(ref _languageIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_LANGUAGEINDEX] = _languageIndex;
            }
        }

        /// <summary>
        /// 物品页是否开启模糊搜索
        /// </summary>
        public bool ItemsSearchFuzzy
        {
            get
            {
                try
                {
                    if (_itemsSearchFuzzy == null)
                    {
                        if (_localSettings.Values[SETTING_NAME_SEARCHMODE] == null)
                        {
                            _itemsSearchFuzzy = false;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHMODE].ToString() == "True")
                        {
                            _itemsSearchFuzzy = true;
                        }
                        else if (_localSettings.Values[SETTING_NAME_SEARCHMODE].ToString() == "False")
                        {
                            _itemsSearchFuzzy = false;
                        }
                        else
                        {
                            _itemsSearchFuzzy = false;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                if (_itemsSearchFuzzy == null) _itemsSearchFuzzy = false;
                return _itemsSearchFuzzy ?? false;
            }
            set
            {
                SetProperty(ref _itemsSearchFuzzy, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_SEARCHMODE] = _itemsSearchFuzzy;
            }
        }

        /// <summary>
        /// SteamID
        /// </summary>
        public string SteamID
        {
            get
            {
                try
                {
                    if (_steamID == null)
                    {
                        if (_localSettings.Values[SETTING_NAME_STEAMID] == null)
                        {
                            _steamID = "";
                        }
                        else
                        {
                            string steamId = _localSettings.Values[SETTING_NAME_STEAMID].ToString();
                            _steamID = steamId;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
                if (_steamID == null) _steamID = "";
                return _steamID == null ? "" : _steamID;
            }
            set
            {
                SetProperty(ref _steamID, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_NAME_STEAMID] = _steamID;
            }
        }
    }
}
