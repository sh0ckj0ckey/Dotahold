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

        public List<LineSeries> RadiantAdvantage { get; private set; } = [];

        public MatchDataModel(DotaMatchDataModel matchData)
        {
            this.DotaMatchData = matchData;

            this.StartDateTime = MatchDataHelper.GetWhen(this.DotaMatchData.start_time);
            this.Duration = MatchDataHelper.GetHowLong(this.DotaMatchData.duration);
            this.FirstBloodTime = MatchDataHelper.GetHowLong(this.DotaMatchData.first_blood_time);
            this.GameMode = MatchDataHelper.GetGameMode(this.DotaMatchData.game_mode.ToString());
            this.LobbyType = MatchDataHelper.GetLobbyType(this.DotaMatchData.lobby_type.ToString());

            if (this.DotaMatchData.radiant_gold_adv?.Length > 0)
            {
                this.RadiantAdvantage.Add(new LineSeries
                {
                    Title = "Radiant Advantage",
                    Data = this.DotaMatchData.radiant_gold_adv,
                    LineColor = Windows.UI.Colors.Green,
                });
            }
        }
    }
}
