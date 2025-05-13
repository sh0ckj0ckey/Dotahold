using System;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.Storage;

namespace Dotahold.Data.DataShop
{
    /// <summary>
    /// 这是一只信使，帮你运送你需要的设置项
    /// </summary>
    public partial class SettingsCourier : ObservableObject
    {
        private const string SETTING_APPEARANCE = "Appearance";
        private const string SETTING_STARTUP = "StartupPage";
        private const string SETTING_CDN = "ImageSourceCDN";
        private const string SETTING_LANGUAGE = "Language";
        private const string SETTING_STEAMID = "SteamID";

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        public event EventHandler<int>? AppearanceSettingChanged = null;

        private int _appearanceIndex = -1;
        private int _startupPageIndex = -1;
        private int _imageSourceCDNIndex = -1;
        private int _languageIndex = -1;
        private string? _steamID = null;

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
                        if (_localSettings.Values[SETTING_APPEARANCE] is null)
                        {
                            _appearanceIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_APPEARANCE]?.ToString() == "1")
                        {
                            _appearanceIndex = 1;
                        }
                        else
                        {
                            _appearanceIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
                if (_appearanceIndex < 0) _appearanceIndex = 0;
                return _appearanceIndex < 0 ? 0 : _appearanceIndex;
            }
            set
            {
                SetProperty(ref _appearanceIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_APPEARANCE] = _appearanceIndex;
                AppearanceSettingChanged?.Invoke(this, _appearanceIndex);
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
                        if (_localSettings.Values[SETTING_STARTUP] == null)
                        {
                            _startupPageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_STARTUP]?.ToString() == "1")
                        {
                            _startupPageIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_STARTUP]?.ToString() == "2")
                        {
                            _startupPageIndex = 2;
                        }
                        else
                        {
                            _startupPageIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
                if (_startupPageIndex < 0) _startupPageIndex = 0;
                return _startupPageIndex < 0 ? 0 : _startupPageIndex;
            }
            set
            {
                SetProperty(ref _startupPageIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_STARTUP] = _startupPageIndex;
            }
        }

        /// <summary>
        /// 图片源
        /// </summary>
        public int ImageSourceCDNIndex
        {
            get
            {
                try
                {
                    if (_imageSourceCDNIndex < 0)
                    {
                        if (_localSettings.Values[SETTING_CDN] == null)
                        {
                            _imageSourceCDNIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_CDN]?.ToString() == "1")
                        {
                            _imageSourceCDNIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_CDN]?.ToString() == "2")
                        {
                            _imageSourceCDNIndex = 2;
                        }
                        else
                        {
                            _imageSourceCDNIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
                if (_imageSourceCDNIndex < 0) _imageSourceCDNIndex = 0;
                return _imageSourceCDNIndex < 0 ? 0 : _imageSourceCDNIndex;
            }
            set
            {
                SetProperty(ref _imageSourceCDNIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_CDN] = _imageSourceCDNIndex;
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
                        if (_localSettings.Values[SETTING_LANGUAGE] == null)
                        {
                            _languageIndex = 0;
                        }
                        else if (_localSettings.Values[SETTING_LANGUAGE].ToString() == "1")
                        {
                            _languageIndex = 1;
                        }
                        else if (_localSettings.Values[SETTING_LANGUAGE].ToString() == "2")
                        {
                            _languageIndex = 2;
                        }
                        else
                        {
                            _languageIndex = 0;
                        }
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
                if (_languageIndex < 0) _languageIndex = 0;
                return _languageIndex < 0 ? 0 : _languageIndex;
            }
            set
            {
                SetProperty(ref _languageIndex, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_LANGUAGE] = _languageIndex;
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
                    if (_steamID is null)
                    {
                        if (_localSettings.Values[SETTING_STEAMID] is null)
                        {
                            _steamID = "";
                        }
                        else
                        {
                            _steamID = _localSettings.Values[SETTING_STEAMID]?.ToString() ?? "";
                        }
                    }
                }
                catch (Exception ex) { LogCourier.Log(ex.Message, LogCourier.LogType.Error); }
                _steamID ??= "";
                return _steamID;
            }
            set
            {
                SetProperty(ref _steamID, value);
                ApplicationData.Current.LocalSettings.Values[SETTING_STEAMID] = _steamID;
            }
        }
    }
}
