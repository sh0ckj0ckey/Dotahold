using System;
using System.Collections.Generic;
using Dotahold.Data.Models;
using Dotahold.Helpers;

namespace Dotahold.Models
{
    public class MatchDataModel
    {
        public DotaMatchDataModel DotaMatchData { get; private set; }

        public string StartDateTime { get; private set; }

        public string Duration { get; private set; }

        public string FirstBloodTime { get; private set; }

        public string GameMode { get; private set; }

        public string LobbyType { get; private set; }

        public List<MatchBanPickModel> PicksBans { get; private set; } = [];

        public List<LineSeries> RadiantAdvantage { get; private set; } = [];

        public MatchDataModel(DotaMatchDataModel matchData,
            Func<string, HeroModel?> getHeroById,
            Func<string, ItemModel?> getItemByName,
            Func<string, AbilitiesModel?> getAbilitiesByHeroName,
            Func<string, string> getAbilityNameById,
            Func<string, string> getPermanentBuffNameById)
        {
            this.DotaMatchData = matchData;

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
                    Title = "Radiant Gold Advantage",
                    Data = this.DotaMatchData.radiant_gold_adv,
                    LineColor = Windows.UI.Colors.Goldenrod,
                });
            }

            if (this.DotaMatchData.radiant_xp_adv?.Length > 0)
            {
                this.RadiantAdvantage.Add(new LineSeries
                {
                    Title = "Radiant XP Advantage",
                    Data = this.DotaMatchData.radiant_xp_adv,
                    LineColor = Windows.UI.Colors.MediumOrchid,
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
