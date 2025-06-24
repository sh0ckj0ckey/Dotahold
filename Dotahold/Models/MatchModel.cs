using System;
using Dotahold.Data.Models;
using Dotahold.Helpers;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class MatchModel(DotaMatchModel dotaMatch, HeroModel hero, AbilitiesFacetModel? abilitiesFacet)
    {
        public DotaMatchModel DotaMatch { get; private set; } = dotaMatch;

        public HeroModel Hero { get; private set; } = hero;

        public AbilitiesFacetModel? AbilitiesFacet { get; private set; } = abilitiesFacet;

        public bool PlayerWin { get; private set; } = (dotaMatch.radiant_win && dotaMatch.player_slot < 128) || (!dotaMatch.radiant_win && dotaMatch.player_slot >= 128);

        public string TimeAgo { get; private set; } = MatchDataHelper.GetHowLongAgo(dotaMatch.start_time);

        public string Duration { get; private set; } = MatchDataHelper.GetHowLong(dotaMatch.duration);

        public double KDA { get; private set; } = dotaMatch.deaths > 0 ? Math.Floor(((double)(dotaMatch.kills + dotaMatch.assists) / dotaMatch.deaths) * 10) / 10 : Math.Floor((double)(dotaMatch.kills + dotaMatch.assists) * 10) / 10;

        public string GameMode { get; private set; } = MatchDataHelper.GetGameMode(dotaMatch.game_mode.ToString());

        public string LobbyType { get; private set; } = MatchDataHelper.GetLobbyType(dotaMatch.lobby_type.ToString());
    }

    public class RecentMatchModel : MatchModel
    {
        /// <summary>
        /// 默认英雄全身图片
        /// </summary>
        private static BitmapImage? _defaultHeroImageSource240 = null;

        public AsyncImage HeroImage { get; private set; }

        public RecentMatchModel(DotaMatchModel dotaMatch, HeroModel hero, AbilitiesFacetModel? abilitiesFacet) : base(dotaMatch, hero, abilitiesFacet)
        {
            _defaultHeroImageSource240 ??= new BitmapImage(new Uri("ms-appx:///Assets/Aegis.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 240
            };

            this.HeroImage = new AsyncImage($"{Dotahold.Data.DataShop.ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/heroes/crops/{hero.DotaHeroAttributes.name.Replace("npc_dota_hero_", "")}.png", 0, 240, _defaultHeroImageSource240);
        }
    }
}
