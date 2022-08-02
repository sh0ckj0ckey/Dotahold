using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace OpenDota_UWP.Converters
{
    internal class IntToGameModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string gameMode = value.ToString();
                    switch (gameMode)
                    {
                        case "0":
                        case "game_mode_unknown":
                            return "Unknown";

                        case "1":
                        case "game_mode_all_pick":
                            return "All Pick";

                        case "2":
                        case "game_mode_captains_mode":
                            return "Captains Mode";

                        case "3":
                        case "game_mode_random_draft":
                            return "Random Draft";

                        case "4":
                        case "game_mode_single_draft":
                            return "Single Draft";

                        case "5":
                        case "game_mode_all_random":
                            return "All Random";

                        case "6":
                        case "game_mode_intro":
                            return "Intro";

                        case "7":
                        case "game_mode_diretide":
                            return "Diretide";

                        case "8":
                        case "game_mode_reverse_captains_mode":
                            return "Reverse Captains Mode";

                        case "9":
                        case "game_mode_greeviling":
                            return "Greeviling";

                        case "10":
                        case "game_mode_tutorial":
                            return "Tutorial";

                        case "11":
                        case "game_mode_mid_only":
                            return "Mid Only";

                        case "12":
                        case "game_mode_least_played":
                            return "Least Played";

                        case "13":
                        case "game_mode_limited_heroes":
                            return "Limited Heroes";

                        case "14":
                        case "game_mode_compendium_matchmaking":
                            return "Compendium Matchmaking";

                        case "15":
                        case "game_mode_custom":
                            return "Custom";

                        case "16":
                        case "game_mode_captains_draft":
                            return "Captains Draft";

                        case "17":
                        case "game_mode_balanced_draft":
                            return "Balanced Draft";

                        case "18":
                        case "game_mode_ability_draft":
                            return "Ability Draft";

                        case "19":
                        case "game_mode_event":
                            return "Event";

                        case "20":
                        case "game_mode_all_random_death_match":
                            return "All Random Death Match";

                        case "21":
                        case "game_mode_1v1_mid":
                            return "1v1 Mid";

                        case "22":
                        case "game_mode_all_draft":
                            return "All Draft";

                        case "23":
                        case "game_mode_turbo":
                            return "Turbo";

                        case "24":
                        case "game_mode_mutation":
                            return "Mutation";

                        default:
                            return gameMode;
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}