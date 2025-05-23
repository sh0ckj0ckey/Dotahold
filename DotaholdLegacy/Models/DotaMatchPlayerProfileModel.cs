﻿using System;
using System.Threading.Tasks;
using Dotahold.Data.DataShop;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class DotaMatchPlayerProfileModel
    {
        public object solo_competitive_rank { get; set; }

        /// <summary>
        /// 冠绝一世排名
        /// </summary>
        public int? leaderboard_rank { get; set; }

        public Mmr_Estimate mmr_estimate { get; set; }
        public object competitive_rank { get; set; }

        /// <summary>
        /// 分段，十位数表示徽章，个位数表示星级
        /// </summary>
        public int? rank_tier { get; set; }
        public Profile profile { get; set; }
    }

    public class Mmr_Estimate
    {
        public int estimate { get; set; }
    }

    public class Profile : ViewModelBase
    {
        public long account_id { get; set; }
        public string personaname { get; set; }
        public object name { get; set; }
        public bool? plus { get; set; }
        public object cheese { get; set; }
        public string steamid { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string profileurl { get; set; }
        public object last_login { get; set; }
        public object loccountrycode { get; set; }
        public object status { get; set; }
        public object is_contributor { get; set; }
        public object is_subscriber { get; set; }

        // 玩家头像
        [JsonIgnore]
        private BitmapImage _AvatarSource = ConstantsCourier.DefaultAvatarImageSource72;
        [JsonIgnore]
        public BitmapImage AvatarSource
        {
            get { return _AvatarSource; }
            private set { Set("AvatarSource", ref _AvatarSource, value); }
        }
        [JsonIgnore]
        private bool _loadedAvatar = false;
        public async Task LoadAvatarAsync(int decodeWidth)
        {
            try
            {
                if (_loadedAvatar || string.IsNullOrWhiteSpace(this.avatarfull)) return;
                var avatarSource = await ImageCourier.GetImageAsync(this.avatarfull, decodeWidth, 0, false);
                if (avatarSource != null)
                {
                    this.AvatarSource = avatarSource;
                    _loadedAvatar = true;
                }
            }
            catch (Exception ex) { LogCourier.LogAsync(ex.Message, LogCourier.LogType.Error); }
        }
    }

}
