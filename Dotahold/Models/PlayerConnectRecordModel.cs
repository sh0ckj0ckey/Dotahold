using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class PlayerConnectRecordModel
    {
        /// <summary>
        /// 默认头像图片
        /// </summary>
        private static BitmapImage? _defaultAvatarImageSource96 = null;

        public AsyncImage AvatarImage { get; private set; }

        public string SteamId { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public PlayerConnectRecordModel(string steamId, string avatar, string name)
        {
            _defaultAvatarImageSource96 ??= new BitmapImage(new Uri("ms-appx:///Assets/Profile/img_default_avatar.jpeg"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 96
            };

            this.SteamId = steamId;
            this.Avatar = avatar;
            this.Name = name;
            this.AvatarImage = new AsyncImage(avatar, 0, 96, _defaultAvatarImageSource96);
        }
    }

    public struct PlayerConnectRecord()
    {
        public string SteamId { get; set; } = string.Empty;

        public string Avatar { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
    }
}
