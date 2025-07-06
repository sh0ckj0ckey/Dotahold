using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<MatchPlayerRatioModel> RadiantPlayerDamageRatios { get; private set; }

        public List<MatchPlayerRatioModel> DirePlayerDamageRatios { get; private set; }

        public List<MatchPlayerRatioModel> RadiantPlayerTeamfightRatios { get; private set; }

        public List<MatchPlayerRatioModel> DirePlayerTeamfightRatios { get; private set; }

        public MatchDataModel(DotaMatchDataModel matchData,
            Func<int, HeroModel?> getHeroById,
            Func<string, ItemModel?> getItemByName,
            Func<int, ItemModel?> getItemById,
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
                    var hero = getHeroById(banPick.hero_id);
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
                    var playerModel = new MatchPlayerModel(player, this.DotaMatchData.od_data?.has_parsed ?? true, getHeroById, getItemByName, getItemById, getAbilitiesByHeroName, getAbilityNameById, getPermanentBuffNameById);

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

            // Player Damage Ratios
            {
                double radiantTotalDamage = this.RadiantPlayers.Sum(player => player.DotaMatchPlayer.hero_damage);
                this.RadiantPlayerDamageRatios = [.. this.RadiantPlayers
                        .Select(player => new MatchPlayerRatioModel(
                            player.Hero,
                            player.SlotColorBrush,
                            radiantTotalDamage > 0 ? Math.Floor(1000 * (player.DotaMatchPlayer.hero_damage / radiantTotalDamage)) / 10 : 0.0
                        )).OrderByDescending(player => player.Value)];

                double direTotalDamage = this.DirePlayers.Sum(player => player.DotaMatchPlayer.hero_damage);
                this.DirePlayerDamageRatios = [.. this.DirePlayers
                        .Select(player => new MatchPlayerRatioModel(
                            player.Hero,
                            player.SlotColorBrush,
                            direTotalDamage > 0 ? Math.Floor(1000 * (player.DotaMatchPlayer.hero_damage / direTotalDamage)) / 10 : 0.0
                        )).OrderByDescending(player => player.Value)];

                double radiantTotalKills = this.RadiantPlayers.Sum(player => player.DotaMatchPlayer.kills);
                this.RadiantPlayerTeamfightRatios = [.. this.RadiantPlayers
                        .Select(player => new MatchPlayerRatioModel(
                            player.Hero,
                            player.SlotColorBrush,
                            radiantTotalKills > 0 ? Math.Floor(1000 * ((player.DotaMatchPlayer.kills + player.DotaMatchPlayer.assists) / radiantTotalKills)) / 10 : 0.0
                        )).OrderByDescending(player => player.Value)];

                double direTotalKills = this.DirePlayers.Sum(player => player.DotaMatchPlayer.kills);
                this.DirePlayerTeamfightRatios = [.. this.DirePlayers
                        .Select(player => new MatchPlayerRatioModel(
                            player.Hero,
                            player.SlotColorBrush,
                            direTotalKills > 0 ? Math.Floor(1000 * ((player.DotaMatchPlayer.kills + player.DotaMatchPlayer.assists) / direTotalKills)) / 10 : 0.0
                        )).OrderByDescending(player => player.Value)];
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

        public SolidColorBrush SlotColorBrush { get; private set; }

        public bool HasParsed { get; private set; }

        public ItemModel? Item0 { get; private set; }

        public ItemModel? Item1 { get; private set; }

        public ItemModel? Item2 { get; private set; }

        public ItemModel? Item3 { get; private set; }

        public ItemModel? Item4 { get; private set; }

        public ItemModel? Item5 { get; private set; }

        public ItemModel? Backpack0 { get; private set; }

        public ItemModel? Backpack1 { get; private set; }

        public ItemModel? Backpack2 { get; private set; }

        public ItemModel? ItemNeutral { get; private set; }

        public ItemModel? ItemNeutralEnhancement { get; private set; }

        public bool HasAghanimScepter { get; private set; }

        public bool HasAghanimShard { get; private set; }

        public MatchPlayerAdditionalUnitModel? AdditionalUnit { get; private set; } = null;

        public MatchPlayerModel(DotaMatchPlayer player, bool hasParsed,
            Func<int, HeroModel?> getHeroById,
            Func<string, ItemModel?> getItemByName,
            Func<int, ItemModel?> getItemById,
            Func<string, AbilitiesModel?> getAbilitiesByHeroName,
            Func<string, string> getAbilityNameById,
            Func<string, string> getPermanentBuffNameById)
        {
            this.DotaMatchPlayer = player;
            this.Hero = getHeroById(this.DotaMatchPlayer.hero_id);
            this.AbilitiesFacet = this.Hero is not null ? getAbilitiesByHeroName(this.Hero.DotaHeroAttributes.name)?.GetFacetByIndex(this.DotaMatchPlayer.hero_variant) : null;
            this.SlotColorBrush = MatchDataHelper.GetSlotColorBrush(this.DotaMatchPlayer.player_slot);
            this.HasParsed = hasParsed;

            this.Item0 = getItemById(this.DotaMatchPlayer.item_0);
            this.Item1 = getItemById(this.DotaMatchPlayer.item_1);
            this.Item2 = getItemById(this.DotaMatchPlayer.item_2);
            this.Item3 = getItemById(this.DotaMatchPlayer.item_3);
            this.Item4 = getItemById(this.DotaMatchPlayer.item_4);
            this.Item5 = getItemById(this.DotaMatchPlayer.item_5);
            this.Backpack0 = getItemById(this.DotaMatchPlayer.backpack_0);
            this.Backpack1 = getItemById(this.DotaMatchPlayer.backpack_1);
            this.Backpack2 = getItemById(this.DotaMatchPlayer.backpack_2);
            this.ItemNeutral = getItemById(this.DotaMatchPlayer.item_neutral);
            this.ItemNeutralEnhancement = getItemById(this.DotaMatchPlayer.item_neutral2);
            this.HasAghanimScepter = this.DotaMatchPlayer.aghanims_scepter > 0;
            this.HasAghanimShard = this.DotaMatchPlayer.aghanims_shard > 0;

            if (this.DotaMatchPlayer.additional_units?.Length > 0)
            {
                this.AdditionalUnit = new MatchPlayerAdditionalUnitModel(this.DotaMatchPlayer.additional_units[0], getItemById);
            }
        }

    }

    public class MatchPlayerRatioModel(HeroModel? hero, SolidColorBrush solidColorBrush, double value)
    {
        public HeroModel? Hero { get; private set; } = hero;

        public SolidColorBrush SlotColorBrush { get; private set; } = solidColorBrush;

        public double Value { get; private set; } = value;
    }

    public class MatchPlayerAdditionalUnitModel
    {
        /// <summary>
        /// 附加单位-熊灵 的图标
        /// </summary>
        private static BitmapImage? _additionalUnitSpiritBearIconSource40 = null;

        /// <summary>
        /// 附加单位-熊灵 的肖像图
        /// </summary>
        private static BitmapImage? _additionalUnitSpiritBearImageSource96 = null;

        public AsyncImage AdditionalUnitIcon { get; private set; }

        public AsyncImage AdditionalUnitImage { get; private set; }

        public string AdditionalUnitName { get; private set; }

        public ItemModel? Item0 { get; private set; }

        public ItemModel? Item1 { get; private set; }

        public ItemModel? Item2 { get; private set; }

        public ItemModel? Item3 { get; private set; }

        public ItemModel? Item4 { get; private set; }

        public ItemModel? Item5 { get; private set; }

        public ItemModel? Backpack0 { get; private set; }

        public ItemModel? Backpack1 { get; private set; }

        public ItemModel? Backpack2 { get; private set; }

        public ItemModel? ItemNeutral { get; private set; }

        public MatchPlayerAdditionalUnitModel(DotaMatchPlayerAdditionalUnit additionalUnit, Func<int, ItemModel?> getItemById)
        {
            if (additionalUnit.unitname == "spirit_bear")
            {
                _additionalUnitSpiritBearIconSource40 ??= new BitmapImage(new Uri("ms-appx:///Assets/Matches/icon_spirit_bear.png"))
                {
                    DecodePixelType = DecodePixelType.Logical,
                    DecodePixelHeight = 40
                };

                _additionalUnitSpiritBearImageSource96 ??= new BitmapImage(new Uri("ms-appx:///Assets/Matches/img_spirit_bear_large.png"))
                {
                    DecodePixelType = DecodePixelType.Logical,
                    DecodePixelHeight = 96
                };

                this.AdditionalUnitIcon = new AsyncImage(string.Empty, 0, 32, _additionalUnitSpiritBearIconSource40);
                this.AdditionalUnitImage = new AsyncImage(string.Empty, 0, 96, _additionalUnitSpiritBearImageSource96);
                this.AdditionalUnitName = "Spirit Bear";
            }
            else
            {
                this.AdditionalUnitIcon = new AsyncImage(string.Empty, 0, 40);
                this.AdditionalUnitImage = new AsyncImage(string.Empty, 0, 96);
                this.AdditionalUnitName = additionalUnit.unitname.Replace("_", " ");
            }

            this.Item0 = getItemById(additionalUnit.item_0);
            this.Item1 = getItemById(additionalUnit.item_1);
            this.Item2 = getItemById(additionalUnit.item_2);
            this.Item3 = getItemById(additionalUnit.item_3);
            this.Item4 = getItemById(additionalUnit.item_4);
            this.Item5 = getItemById(additionalUnit.item_5);
            this.Backpack0 = getItemById(additionalUnit.backpack_0);
            this.Backpack1 = getItemById(additionalUnit.backpack_1);
            this.Backpack2 = getItemById(additionalUnit.backpack_2);
            this.ItemNeutral = getItemById(additionalUnit.item_neutral);
        }
    }
}
