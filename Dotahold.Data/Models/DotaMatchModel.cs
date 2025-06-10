using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaMatchModel
    {
        [JsonConverter(typeof(SafeLongConverter))]
        public long match_id { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int player_slot { get; set; }

        [JsonConverter(typeof(SafeBoolConverter))]
        public bool radiant_win { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_id { get; set; }

        [JsonConverter(typeof(SafeLongConverter))]
        public long start_time { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int duration { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int game_mode { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int lobby_type { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int kills { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int deaths { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int assists { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int average_rank { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int xp_per_min { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int gold_per_min { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_damage { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int tower_damage { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_healing { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int last_hits { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_variant { get; set; }
    }
}
