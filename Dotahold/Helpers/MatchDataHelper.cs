using System;
using Dotahold.Data.DataShop;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Dotahold.Helpers
{
    public static class MatchDataHelper
    {
        public static string GetWhen(long startTime)
        {
            try
            {
                DateTimeOffset dateTimeOffset = startTime.ToString().Length == 13 ? DateTimeOffset.FromUnixTimeMilliseconds(startTime) : DateTimeOffset.FromUnixTimeSeconds(startTime);
                return dateTimeOffset.LocalDateTime.ToString("yyyy-MM-dd HH:mm");
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Compute DateTime({startTime}) error: {ex.Message}", LogCourier.LogType.Error);
            }

            return string.Empty;
        }

        public static string GetHowLongAgo(long startTime)
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

        public static string GetHowLong(long duration)
        {
            try
            {
                long minutes = duration / 60;
                long seconds = duration % 60;
                return $"{minutes:D2}:{seconds:D2}";
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Compute HowLong({duration}) error: {ex.Message}", LogCourier.LogType.Error);
            }

            return "00:00";
        }

        public static string GetGameMode(string gameMode)
        {
            return gameMode switch
            {
                "0" or "game_mode_unknown" => "Unknown",
                "1" or "game_mode_all_pick" => "All Pick",
                "2" or "game_mode_captains_mode" => "Captains Mode",
                "3" or "game_mode_random_draft" => "Random Draft",
                "4" or "game_mode_single_draft" => "Single Draft",
                "5" or "game_mode_all_random" => "All Random",
                "6" or "game_mode_intro" => "Intro",
                "7" or "game_mode_diretide" => "Diretide",
                "8" or "game_mode_reverse_captains_mode" => "Reverse Captains Mode",
                "9" or "game_mode_greeviling" => "Greeviling",
                "10" or "game_mode_tutorial" => "Tutorial",
                "11" or "game_mode_mid_only" => "Mid Only",
                "12" or "game_mode_least_played" => "Least Played",
                "13" or "game_mode_limited_heroes" => "Limited Heroes",
                "14" or "game_mode_compendium_matchmaking" => "Compendium Matchmaking",
                "15" or "game_mode_custom" => "Custom",
                "16" or "game_mode_captains_draft" => "Captains Draft",
                "17" or "game_mode_balanced_draft" => "Balanced Draft",
                "18" or "game_mode_ability_draft" => "Ability Draft",
                "19" or "game_mode_event" => "Event",
                "20" or "game_mode_all_random_death_match" => "All Random Death Match",
                "21" or "game_mode_1v1_mid" => "1v1 Mid",
                "22" or "game_mode_all_draft" => "All Draft",
                "23" or "game_mode_turbo" => "Turbo",
                "24" or "game_mode_mutation" => "Mutation",
                "25" or "game_mode_coaches_challenge" => "Coaches Challenge",
                _ => gameMode.Replace("game_mode_", "").Replace("_", " ").ToUpper(),
            };
        }

        public static string GetLobbyType(string lobbyType)
        {
            return lobbyType switch
            {
                "0" or "lobby_type_normal" => "Normal",
                "1" or "lobby_type_practice" => "Practice",
                "2" or "lobby_type_tournament" => "Tournament",
                "3" or "lobby_type_tutorial" => "Tutorial",
                "4" or "lobby_type_coop_bots" => "Coop Bots",
                "5" or "lobby_type_ranked_team_mm" => "Ranked Team",
                "6" or "lobby_type_ranked_solo_mm" => "Ranked Solo",
                "7" or "lobby_type_ranked" => "Ranked",
                "8" or "lobby_type_1v1_mid" => "1v1 Mid",
                "9" or "lobby_type_battle_cup" => "Battle Cup",
                "10" or "lobby_type_local_bots" => "Local Bots",
                "11" or "lobby_type_spectator" => "Spectator",
                "12" or "lobby_type_event" => "Event",
                "13" or "lobby_type_gauntlet" => "Gauntlet",
                "14" or "lobby_type_new_player" => "New Player",
                "15" or "lobby_type_featured" => "Featured",
                _ => lobbyType.Replace("lobby_type_", "").Replace("_", " ").ToUpper(),
            };
        }

        private static SolidColorBrush? _slot0ColorBrush;
        private static SolidColorBrush? _slot1ColorBrush;
        private static SolidColorBrush? _slot2ColorBrush;
        private static SolidColorBrush? _slot3ColorBrush;
        private static SolidColorBrush? _slot4ColorBrush;
        private static SolidColorBrush? _slot128ColorBrush;
        private static SolidColorBrush? _slot129ColorBrush;
        private static SolidColorBrush? _slot130ColorBrush;
        private static SolidColorBrush? _slot131ColorBrush;
        private static SolidColorBrush? _slot132ColorBrush;
        private static SolidColorBrush? _slotXColorBrush;

        public static SolidColorBrush GetSlotColorBrush(int slot)
        {
            switch (slot)
            {
                case 0:
                    {
                        _slot0ColorBrush ??= new(new Color() { R = 51, G = 117, B = 255, A = 255 });
                        return _slot0ColorBrush;
                    }
                case 1:
                    {
                        _slot1ColorBrush ??= new(new Color() { R = 102, G = 255, B = 191, A = 255 });
                        return _slot1ColorBrush;
                    }
                case 2:
                    {
                        _slot2ColorBrush ??= new(new Color() { R = 191, G = 0, B = 191, A = 255 });
                        return _slot2ColorBrush;
                    }
                case 3:
                    {
                        _slot3ColorBrush ??= new(new Color() { R = 243, G = 240, B = 11, A = 255 });
                        return _slot3ColorBrush;
                    }
                case 4:
                    {
                        _slot4ColorBrush ??= new(new Color() { R = 255, G = 107, B = 0, A = 255 });
                        return _slot4ColorBrush;
                    }
                case 128:
                    {
                        _slot128ColorBrush ??= new(new Color() { R = 254, G = 134, B = 194, A = 255 });
                        return _slot128ColorBrush;
                    }
                case 129:
                    {
                        _slot129ColorBrush ??= new(new Color() { R = 161, G = 180, B = 71, A = 255 });
                        return _slot129ColorBrush;
                    }
                case 130:
                    {
                        _slot130ColorBrush ??= new(new Color() { R = 101, G = 217, B = 247, A = 255 });
                        return _slot130ColorBrush;
                    }
                case 131:
                    {
                        _slot131ColorBrush ??= new(new Color() { R = 0, G = 131, B = 33, A = 255 });
                        return _slot131ColorBrush;
                    }
                case 132:
                    {
                        _slot132ColorBrush ??= new(new Color() { R = 164, G = 105, B = 0, A = 255 });
                        return _slot132ColorBrush;
                    }
                default:
                    {
                        _slotXColorBrush ??= new(new Color() { R = 128, G = 128, B = 128, A = 255 });
                        return _slotXColorBrush;
                    }
            }
        }
    }
}
