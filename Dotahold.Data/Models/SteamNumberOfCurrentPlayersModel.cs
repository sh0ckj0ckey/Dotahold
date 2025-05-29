using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class SteamNumberOfCurrentPlayersResponse
    {
        public SteamNumberOfCurrentPlayersModel? response { get; set; }
    }

    public class SteamNumberOfCurrentPlayersModel
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int player_count { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int result { get; set; }
    }
}
