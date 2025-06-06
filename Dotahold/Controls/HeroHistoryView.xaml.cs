﻿using System;
using Dotahold.Data.DataShop;
using Dotahold.Models;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace Dotahold.Controls
{
    public sealed partial class HeroHistoryView : UserControl
    {
        private readonly HeroModel _heroModel;

        private readonly string _historyContent;

        [System.Text.RegularExpressions.GeneratedRegex("&[^;]+;")]
        private static partial System.Text.RegularExpressions.Regex RegexSpecialCharacters();

        [System.Text.RegularExpressions.GeneratedRegex("<[^>]+>")]
        private static partial System.Text.RegularExpressions.Regex RegexHTMLTags();

        public HeroHistoryView(HeroModel heroModel, string history)
        {
            _heroModel = heroModel;
            _historyContent = FormatHeroHistory(history);

            this.InitializeComponent();
        }

        /// <summary>
        /// 处理英雄背景故事字符串，去掉包含的一些标签和多余的转义符
        /// </summary>
        /// <param name="history"></param>
        /// <returns></returns>
        private static string FormatHeroHistory(string history)
        {
            try
            {
                string strText = RegexHTMLTags().Replace(history, "");
                strText = RegexSpecialCharacters().Replace(strText, "");
                strText = strText.Replace("\t", "");
                strText = strText.Replace("\r", "\n");
                return strText;
            }
            catch (Exception ex)
            {
                LogCourier.Log(ex.Message, LogCourier.LogType.Error);
            }

            return history;
        }
    }
}
