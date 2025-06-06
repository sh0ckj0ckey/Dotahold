using System;
using Dotahold.Data.Models;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class PlayerProfileModel
    {
        /// <summary>
        /// 默认头像图片
        /// </summary>
        private static BitmapImage? _defaultAvatarImageSource96 = null;

        public DotaPlayerProfileModel DotaPlayerProfile { get; private set; }

        public AsyncImage AvatarImage { get; private set; }

        public PlayerProfileModel(DotaPlayerProfileModel profile)
        {
            _defaultAvatarImageSource96 ??= new BitmapImage(new Uri("ms-appx:///Assets/Profile/img_default_avatar.jpeg"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 96
            };

            this.DotaPlayerProfile = profile;
            this.AvatarImage = new AsyncImage(this.DotaPlayerProfile.profile?.avatarmedium ?? string.Empty, 0, 96, _defaultAvatarImageSource96);
        }
    }
}
