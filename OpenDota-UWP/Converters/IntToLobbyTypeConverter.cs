using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Dotahold.Converters
{
    internal class IntToLobbyTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                if (value != null)
                {
                    string lobbyType = value.ToString();
                    switch (lobbyType)
                    {
                        case "0":
                        case "lobby_type_normal":
                            return "Normal";

                        case "1":
                        case "lobby_type_practice":
                            return "Practice";

                        case "2":
                        case "lobby_type_tournament":
                            return "Tournament";

                        case "3":
                        case "lobby_type_tutorial":
                            return "Tutorial";

                        case "4":
                        case "lobby_type_coop_bots":
                            return "Coop Bots";

                        case "5":
                        case "lobby_type_ranked_team_mm":
                            return "Ranked Team";

                        case "6":
                        case "lobby_type_ranked_solo_mm":
                            return "Ranked Solo";

                        case "7":
                        case "lobby_type_ranked":
                            return "Ranked";

                        case "8":
                        case "lobby_type_1v1_mid":
                            return "1v1 Mid";

                        case "9":
                        case "lobby_type_battle_cup":
                            return "Battle Cup";

                        default:
                            return lobbyType;
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