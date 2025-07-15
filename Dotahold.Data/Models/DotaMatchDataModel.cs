using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaMatchDataModel
    {
        [JsonConverter(typeof(SafeLongConverter))]
        public long match_id { get; set; }

        [JsonConverter(typeof(SafeLongConverter))]
        public long start_time { get; set; }

        [JsonConverter(typeof(SafeLongConverter))]
        public long duration { get; set; }

        [JsonConverter(typeof(SafeBoolConverter))]
        public bool radiant_win { get; set; }

        [JsonConverter(typeof(SafeLongConverter))]
        public long first_blood_time { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int lobby_type { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int game_mode { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int radiant_score { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int dire_score { get; set; }

        public int[]? radiant_gold_adv { get; set; }

        public int[]? radiant_xp_adv { get; set; }

        public DotaMatchBanPick[]? picks_bans { get; set; }

        public DotaMatchOpenDotaData? od_data { get; set; } = null;

        public DotaMatchLeague? league { get; set; } = null;

        public DotaMatchTeam? radiant_team { get; set; } = null;

        public DotaMatchTeam? dire_team { get; set; } = null;

        public DotaMatchPlayer[]? players { get; set; }
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

    public class DotaMatchPlayer
    {
        [JsonConverter(typeof(SafeStringConverter))]
        public string account_id { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string personaname { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string name { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeIntConverter))]
        public int rank_tier { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int player_slot { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int party_id { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int party_size { get; set; }


        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_id { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_variant { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_0 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_1 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_2 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_3 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_4 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_5 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int backpack_0 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int backpack_1 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int backpack_2 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_neutral { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_neutral2 { get; set; }


        [JsonConverter(typeof(SafeIntConverter))]
        public int aghanims_scepter { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int aghanims_shard { get; set; }


        [JsonConverter(typeof(SafeIntConverter))]
        public int kills { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int deaths { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int assists { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int last_hits { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int denies { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int gold_per_min { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int xp_per_min { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int level { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int net_worth { get; set; }


        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_damage { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int tower_damage { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_healing { get; set; }


        [JsonConverter(typeof(SafeIntConverter))]
        public int firstblood_claimed { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int rune_pickups { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int buyback_count { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int courier_kills { get; set; }


        [JsonConverter(typeof(SafeIntConverter))]
        public int obs_placed { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int sen_placed { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int observer_kills { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int sentry_kills { get; set; }


        [JsonConverter(typeof(SafeIntConverter))]
        public int creeps_stacked { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int camps_stacked { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int neutral_kills { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int roshan_kills { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int tower_kills { get; set; }


        public int[]? times { get; set; } = [];

        public int[]? gold_t { get; set; } = [];

        public int[]? lh_t { get; set; } = [];

        public int[]? dn_t { get; set; } = [];

        public int[]? xp_t { get; set; } = [];

        public DotaMatchPlayerAdditionalUnit[]? additional_units { get; set; }

        public DotaMatchPlayerLog[]? purchase_log { get; set; }

        public DotaMatchPlayerLog[]? kills_log { get; set; }

        public DotaMatchPlayerLog[]? runes_log { get; set; }

        public int[]? ability_upgrades_arr { get; set; }

        public DotaMatchPlayerNeutralItemHistory[]? neutral_item_history { get; set; }

        public DotaMatchPlayerPermanentBuff[]? permanent_buffs { get; set; }

        public System.Collections.Generic.Dictionary<string, DotaMatchPlayerBenchmark?>? benchmarks { get; set; }
    }

    public class DotaMatchPlayerAdditionalUnit
    {
        [JsonConverter(typeof(SafeStringConverter))]
        public string unitname { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_0 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_1 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_2 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_3 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_4 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_5 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int backpack_0 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int backpack_1 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int backpack_2 { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int item_neutral { get; set; }
    }

    public class DotaMatchPlayerLog
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int time { get; set; }

        [JsonConverter(typeof(SafeStringConverter))]
        public string key { get; set; } = string.Empty;
    }

    public class DotaMatchPlayerNeutralItemHistory
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int time { get; set; }

        [JsonConverter(typeof(SafeStringConverter))]
        public string item_neutral { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string item_neutral_enhancement { get; set; } = string.Empty;
    }

    public class DotaMatchPlayerPermanentBuff
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int permanent_buff { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int stack_count { get; set; }
    }

    public class DotaMatchPlayerBenchmark
    {
        [JsonConverter(typeof(SafeDoubleConverter))]
        public double raw { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double pct { get; set; }
    }
}
