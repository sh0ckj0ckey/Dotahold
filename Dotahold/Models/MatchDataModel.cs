using System;
using System.Collections.Generic;
using Dotahold.Data.Models;
using Dotahold.Helpers;
using Windows.UI;
using Windows.UI.Xaml.Media;
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

        public List<MatchPlayerModel> RadiantPlayers { get; private set; } = [];

        public List<MatchPlayerModel> DirePlayers { get; private set; } = [];

        public List<LineSeries> RadiantAdvantage { get; private set; } = [];

        public List<LineSeries> PlayersGold { get; private set; } = [];

        public List<LineSeries> PlayersExperience { get; private set; } = [];

        public MatchTeamModel? RadiantTeam { get; private set; }

        public MatchTeamModel? DireTeam { get; private set; }

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

            // Ban Pick
            if (this.DotaMatchData.picks_bans is not null)
            {
                foreach (var banPick in this.DotaMatchData.picks_bans)
                {
                    var hero = getHeroById(banPick.hero_id.ToString());
                    if (hero is not null)
                    {
                        this.PicksBans.Add(new MatchBanPickModel(banPick, hero));
                    }
                }
            }

            // Radiant Gold Advantage
            if (this.DotaMatchData.radiant_gold_adv?.Length > 0)
            {
                this.RadiantAdvantage.Add(new LineSeries
                {
                    Icon = new AsyncImage(string.Empty, 0, 0, _defaultGoldImageSource32),
                    Title = "Radiant Gold Adv.",
                    NegativeTitle = "Dire Gold Adv.",
                    LineColorBrush = new SolidColorBrush(Colors.Goldenrod),
                    Data = this.DotaMatchData.radiant_gold_adv,
                });
            }

            // Radiant Experience Advantage
            if (this.DotaMatchData.radiant_xp_adv?.Length > 0)
            {
                this.RadiantAdvantage.Add(new LineSeries
                {
                    Icon = new AsyncImage(string.Empty, 0, 0, _defaultExperienceImageSource32),
                    Title = "Radiant XP Adv.",
                    NegativeTitle = "Dire XP Adv.",
                    LineColorBrush = new SolidColorBrush(Colors.MediumOrchid),
                    Data = this.DotaMatchData.radiant_xp_adv,
                });
            }

            // Players and their Gold & Experience
            if (this.DotaMatchData.players is not null)
            {
                foreach (var player in this.DotaMatchData.players)
                {
                    var playerModel = new MatchPlayerModel(player, getHeroById, getItemByName, getAbilitiesByHeroName, getAbilityNameById, getPermanentBuffNameById);

                    if (playerModel.DotaMatchPlayer.player_slot >= 128)
                    {
                        this.DirePlayers.Add(playerModel);
                    }
                    else
                    {
                        this.RadiantPlayers.Add(playerModel);
                    }

                    if (playerModel.DotaMatchPlayer.gold_t?.Length > 0)
                    {
                        this.PlayersGold.Add(new LineSeries
                        {
                            Icon = playerModel.Hero?.HeroIcon,
                            Title = playerModel.Hero?.DotaHeroAttributes.localized_name ?? $"Unknown Hero {playerModel.DotaMatchPlayer.hero_id}",
                            LineColorBrush = MatchDataHelper.GetSlotColorBrush(playerModel.DotaMatchPlayer.player_slot),
                            Data = playerModel.DotaMatchPlayer.gold_t,
                        });
                    }

                    if (playerModel.DotaMatchPlayer.xp_t?.Length > 0)
                    {
                        this.PlayersExperience.Add(new LineSeries
                        {
                            Icon = playerModel.Hero?.HeroIcon,
                            Title = playerModel.Hero?.DotaHeroAttributes.localized_name ?? $"Unknown Hero {playerModel.DotaMatchPlayer.hero_id}",
                            LineColorBrush = MatchDataHelper.GetSlotColorBrush(playerModel.DotaMatchPlayer.player_slot),
                            Data = playerModel.DotaMatchPlayer.xp_t,
                        });
                    }
                }
            }

            // Radiant Team
            if (this.DotaMatchData.radiant_team is not null)
            {
                this.RadiantTeam = new MatchTeamModel(this.DotaMatchData.radiant_team);
            }

            // Dire Team
            if (this.DotaMatchData.dire_team is not null)
            {
                this.DireTeam = new MatchTeamModel(this.DotaMatchData.dire_team);
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

    public class MatchTeamModel(DotaMatchTeam team)
    {
        public string Name { get; private set; } = team.name ?? string.Empty;

        public AsyncImage LogoImage { get; private set; } = new AsyncImage(team.logo_url ?? string.Empty, 0, 72);
    }

    public class MatchPlayerModel
    {
        public DotaMatchPlayer DotaMatchPlayer { get; private set; }

        public HeroModel? Hero { get; private set; }

        public AbilitiesFacetModel? AbilitiesFacet { get; private set; }

        public SolidColorBrush? SlotColorBrush { get; set; }

        public MatchPlayerModel(DotaMatchPlayer player,
            Func<string, HeroModel?> getHeroById,
            Func<string, ItemModel?> getItemByName,
            Func<string, AbilitiesModel?> getAbilitiesByHeroName,
            Func<string, string> getAbilityNameById,
            Func<string, string> getPermanentBuffNameById)
        {
            this.DotaMatchPlayer = player;
            this.Hero = getHeroById(this.DotaMatchPlayer.hero_id.ToString());
            this.AbilitiesFacet = this.Hero is not null ? getAbilitiesByHeroName(this.Hero.DotaHeroAttributes.name)?.GetFacetByIndex(this.DotaMatchPlayer.hero_variant) : null;
            this.SlotColorBrush = MatchDataHelper.GetSlotColorBrush(this.DotaMatchPlayer.player_slot);
        }

    }
}
