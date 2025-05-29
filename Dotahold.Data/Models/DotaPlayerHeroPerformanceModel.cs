using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaPlayerHeroPerformanceModel
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_id { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int last_played { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int games { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int win { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int with_games { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int with_win { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int against_games { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int against_win { get; set; }
    }
}
