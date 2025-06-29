using System;
using System.Collections.Generic;
using Dotahold.Data.Models;
using Dotahold.Helpers;
using Windows.UI.Xaml.Media.Imaging;

namespace Dotahold.Models
{
    public class MatchDataModel
    {
        /// <summary>
        /// 默认经济图标
        /// </summary>
        private static BitmapImage? _defaultGoldImageSource32 = null;

        /// <summary>
        /// 默认经验图标
        /// </summary>
        private static BitmapImage? _defaultExperienceImageSource32 = null;

        public DotaMatchDataModel DotaMatchData { get; private set; }

        public bool IsLeague { get; private set; } = false;

        public string StartDateTime { get; private set; }

        public string Duration { get; private set; }

        public string FirstBloodTime { get; private set; }

        public string GameMode { get; private set; }

        public string LobbyType { get; private set; }

        public List<MatchBanPickModel> PicksBans { get; private set; } = [];

        public List<LineSeries> RadiantAdvantage { get; private set; } = [];

        public List<LineSeries> PlayersGold { get; private set; } = [];

        public List<LineSeries> PlayersExperience { get; private set; } = [];

        public MatchDataModel(DotaMatchDataModel matchData,
            Func<string, HeroModel?> getHeroById,
            Func<string, ItemModel?> getItemByName,
            Func<string, AbilitiesModel?> getAbilitiesByHeroName,
            Func<string, string> getAbilityNameById,
            Func<string, string> getPermanentBuffNameById)
        {
            _defaultGoldImageSource32 ??= new BitmapImage(new Uri("ms-appx:///Assets/Matches/Data/icon_gold_stack.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 32
            };

            _defaultExperienceImageSource32 ??= new BitmapImage(new Uri("ms-appx:///Assets/Matches/Data/icon_tome_of_knowledge.png"))
            {
                DecodePixelType = DecodePixelType.Logical,
                DecodePixelHeight = 32
            };

            this.DotaMatchData = matchData;

            this.IsLeague = !string.IsNullOrWhiteSpace(this.DotaMatchData.league?.name);
            this.StartDateTime = MatchDataHelper.GetWhen(this.DotaMatchData.start_time);
            this.Duration = MatchDataHelper.GetHowLong(this.DotaMatchData.duration);
            this.FirstBloodTime = MatchDataHelper.GetHowLong(this.DotaMatchData.first_blood_time);
            this.GameMode = MatchDataHelper.GetGameMode(this.DotaMatchData.game_mode.ToString());
            this.LobbyType = MatchDataHelper.GetLobbyType(this.DotaMatchData.lobby_type.ToString());

            if (matchData.picks_bans is not null)
            {
                foreach (var banPick in matchData.picks_bans)
                {
                    var hero = getHeroById(banPick.hero_id.ToString());
                    if (hero is not null)
                    {
                        this.PicksBans.Add(new MatchBanPickModel(banPick, hero));
                    }
                }
            }

            if (this.DotaMatchData.radiant_gold_adv?.Length > 0)
            {
                this.RadiantAdvantage.Add(new LineSeries
                {
                    Icon = new AsyncImage(string.Empty, 0, 0, _defaultGoldImageSource32),
                    Title = "Radiant Gold Adv.",
                    NegativeTitle = "Dire Gold Adv.",
                    LineColor = Windows.UI.Colors.Goldenrod,
                    Data = this.DotaMatchData.radiant_gold_adv,
                });
            }

            if (this.DotaMatchData.radiant_xp_adv?.Length > 0)
            {
                this.RadiantAdvantage.Add(new LineSeries
                {
                    Icon = new AsyncImage(string.Empty, 0, 0, _defaultExperienceImageSource32),
                    Title = "Radiant XP Adv.",
                    NegativeTitle = "Dire XP Adv.",
                    LineColor = Windows.UI.Colors.MediumOrchid,
                    Data = this.DotaMatchData.radiant_xp_adv,
                });
            }
        }
    }

    public class MatchBanPickModel(DotaMatchBanPick banPick, HeroModel hero)
    {
        public HeroModel Hero { get; private set; } = hero;

        public string Order { get; private set; } = $"# {banPick.order + 1}";

        public string Team { get; private set; } = banPick.team == 1 ? "Dire" : "Radiant";

        public string Action { get; private set; } = banPick.is_pick ? "Pick" : "Ban";

        public bool IsBan { get; private set; } = !banPick.is_pick;

        public bool IsRadiant { get; private set; } = banPick.team != 1;
    }
}
