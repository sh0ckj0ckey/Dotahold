using System;
using Dotahold.Data.Models;
using Dotahold.Helpers;

namespace Dotahold.Models
{
    public class PlayerHeroPerformanceModel(DotaPlayerHeroPerformanceModel model, HeroModel hero)
    {
        public int HeroId { get; private set; } = model.hero_id;

        public HeroModel Hero { get; private set; } = hero;

        public string LastPlayed { get; private set; } = MatchDataHelper.GetHowLongAgo(model.last_played);

        public int Games { get; private set; } = model.games;

        public int Win { get; private set; } = model.win;

        public string WinRate { get; private set; } = model.games > 0 ? $"{Math.Floor((double)model.win / model.games * 1000) / 10}%" : "100%";

        public int WithGames { get; private set; } = model.with_games;

        public int WithWin { get; private set; } = model.with_win;

        public string WithWinRate { get; private set; } = model.with_games > 0 ? $"{Math.Floor((double)model.with_win / model.with_games * 1000) / 10}%" : "100%";

        public int AgainstGames { get; private set; } = model.against_games;

        public int AgainstWin { get; private set; } = model.against_win;

        public string AgainstWinRate { get; private set; } = model.against_games > 0 ? $"{Math.Floor((double)model.against_win / model.against_games * 1000) / 10}%" : "100%";

    }
}
