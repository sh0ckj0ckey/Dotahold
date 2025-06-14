using System;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;

namespace Dotahold.Models
{
    public class PlayerHeroPerformanceModel(DotaPlayerHeroPerformanceModel model, HeroModel hero)
    {
        public int HeroId { get; private set; } = model.hero_id;

        public HeroModel Hero { get; private set; } = hero;

        public string LastPlayed { get; private set; } = HowLongAgo(model.last_played);

        public int Games { get; private set; } = model.games;

        public int Win { get; private set; } = model.win;

        public string WinRate { get; private set; } = model.games > 0 ? $"{Math.Floor((double)model.win / model.games * 1000) / 10}%" : "100%";

        public int WithGames { get; private set; } = model.with_games;

        public int WithWin { get; private set; } = model.with_win;

        public string WithWinRate { get; private set; } = model.with_games > 0 ? $"{Math.Floor((double)model.with_win / model.with_games * 1000) / 10}%" : "100%";

        public int AgainstGames { get; private set; } = model.against_games;

        public int AgainstWin { get; private set; } = model.against_win;

        public string AgainstWinRate { get; private set; } = model.against_games > 0 ? $"{Math.Floor((double)model.against_win / model.against_games * 1000) / 10}%" : "100%";

        private static string HowLongAgo(long startTime)
        {
            try
            {
                var matchDate = DateTimeOffset.FromUnixTimeSeconds(startTime).UtcDateTime;
                var now = DateTime.UtcNow;
                var span = now - matchDate;

                if (span.TotalDays >= 365)
                {
                    int years = (int)(span.TotalDays / 365);
                    return years == 1 ? "1 year ago" : $"{years} years ago";
                }

                if (span.TotalDays >= 30)
                {
                    int months = (int)(span.TotalDays / 30);
                    return months == 1 ? "1 month ago" : $"{months} months ago";
                }

                if (span.TotalDays >= 1)
                {
                    int days = (int)span.TotalDays;
                    return days == 1 ? "1 day ago" : $"{days} days ago";
                }

                if (span.TotalHours >= 1)
                {
                    int hours = (int)span.TotalHours;
                    return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
                }

                if (span.TotalMinutes >= 1)
                {
                    int minutes = (int)span.TotalMinutes;
                    return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
                }

                int seconds = (int)span.TotalSeconds;
                return seconds <= 1 ? "just now" : $"{seconds} seconds ago";
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Compute HowLongAgo({startTime}) error: {ex.Message}", LogCourier.LogType.Error);
            }

            return string.Empty;
        }
    }
}
