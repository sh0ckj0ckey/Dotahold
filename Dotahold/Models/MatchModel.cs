using System;
using Dotahold.Data.DataShop;
using Dotahold.Data.Models;

namespace Dotahold.Models
{
    public class MatchModel(DotaMatchModel dotaMatch, HeroModel hero, AbilitiesFacetModel? abilitiesFacet)
    {
        public DotaMatchModel DotaMatch { get; private set; } = dotaMatch;

        public HeroModel Hero { get; private set; } = hero;

        public AbilitiesFacetModel? AbilitiesFacet { get; private set; } = abilitiesFacet;

        public bool PlayerWin { get; private set; } = (dotaMatch.radiant_win && dotaMatch.player_slot < 128) || (!dotaMatch.radiant_win && dotaMatch.player_slot >= 128);

        public string TimeAgo { get; private set; } = HowLongAgo(dotaMatch.start_time);

        public string Duration { get; private set; } = HowLong(dotaMatch.duration);

        public double KDA { get; private set; } = dotaMatch.deaths > 0 ? Math.Floor(((double)(dotaMatch.kills + dotaMatch.assists) / dotaMatch.deaths) * 10) / 10 : Math.Floor((double)(dotaMatch.kills + dotaMatch.assists) * 10) / 10;

        public string GameMode { get; private set; } = GetGameMode(dotaMatch.game_mode.ToString());

        public string LobbyType { get; private set; } = GetLobbyType(dotaMatch.lobby_type.ToString());

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

        private static string HowLong(int duration)
        {
            try
            {
                int minutes = duration / 60;
                int seconds = duration % 60;
                return $"{minutes:D2}:{seconds:D2}";
            }
            catch (Exception ex)
            {
                LogCourier.Log($"Compute HowLong({duration}) error: {ex.Message}", LogCourier.LogType.Error);
            }

            return "00:00";
        }

        private static string GetGameMode(string gameMode)
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

        private static string GetLobbyType(string lobbyType)
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
    }
}
