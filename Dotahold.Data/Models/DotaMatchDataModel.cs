using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaMatchDataModel
    {
        [JsonConverter(typeof(SafeLongConverter))]
        public long match_id { get; set; }

        public int[]? radiant_gold_adv { get; set; } = [];

        public int[]? radiant_xp_adv { get; set; } = [];

        [JsonConverter(typeof(SafeLongConverter))]
        public long start_time { get; set; }

        [JsonConverter(typeof(SafeLongConverter))]
        public long duration { get; set; }

        [JsonConverter(typeof(SafeBoolConverter))]
        public bool radiant_win { get; set; }

        [JsonConverter(typeof(SafeLongConverter))]
        public long first_blood_time { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public long lobby_type { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public long game_mode { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public long radiant_score { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public long dire_score { get; set; }

        public DotaMatchBanPick[]? picks_bans { get; set; } = [];

        public DotaMatchTeam? radiant_team { get; set; } = null;

        public DotaMatchTeam? dire_team { get; set; } = null;
    }

    public class DotaMatchBanPick
    {
        [JsonConverter(typeof(SafeBoolConverter))]
        public bool is_pick { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_id { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int team { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int order { get; set; }
    }

    public class DotaMatchOpenDotaData
    {
        [JsonConverter(typeof(SafeBoolConverter))]
        public bool has_parsed { get; set; }
    }

    public class DotaMatchLeague
    {
        public string? name { get; set; } = string.Empty;
    }

    public class DotaMatchTeam
    {
        public string? name { get; set; } = string.Empty;

        public string? logo_url { get; set; } = string.Empty;
    }
}
