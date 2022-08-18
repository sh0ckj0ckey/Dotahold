using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaMatchInfoModel
    {
        public long? match_id { get; set; }
        public int? dire_score { get; set; }
        public long? duration { get; set; }
        public long? first_blood_time { get; set; }
        public int? game_mode { get; set; }
        public int? lobby_type { get; set; }
        public List<Picks_Bans> picks_bans { get; set; }
        public List<int> radiant_gold_adv { get; set; }
        public int? radiant_score { get; set; }
        public bool? radiant_win { get; set; }
        public List<int> radiant_xp_adv { get; set; }
        public int? skill { get; set; }
        public long? start_time { get; set; }
        public List<Player> players { get; set; }
    }

    public class Picks_Bans
    {
        public bool? is_pick { get; set; }
        public int? hero_id { get; set; }
        //public int team { get; set; }
        //public int order { get; set; }
        //public int ord { get; set; }
        //public long match_id { get; set; }
    }

    public class Player
    {
        public long? match_id { get; set; }
        public int? player_slot { get; set; }
        public List<int> ability_upgrades_arr { get; set; }
        public long? account_id { get; set; }
        public Additional_Units[] additional_units { get; set; }
        public int? assists { get; set; }
        public int? backpack_0 { get; set; }
        public int? backpack_1 { get; set; }
        public int? backpack_2 { get; set; }
        public int? backpack_3 { get; set; }
        public int? camps_stacked { get; set; }
        public int? creeps_stacked { get; set; }
        public int? deaths { get; set; }
        public int? denies { get; set; }
        public int? gold { get; set; }
        public int? gold_per_min { get; set; }
        public int? gold_spent { get; set; }
        public List<int> gold_t { get; set; }
        public int? hero_damage { get; set; }
        public int? hero_healing { get; set; }
        public int? hero_id { get; set; }
        public int? item_0 { get; set; }
        public int? item_1 { get; set; }
        public int? item_2 { get; set; }
        public int? item_3 { get; set; }
        public int? item_4 { get; set; }
        public int? item_5 { get; set; }
        public int? item_neutral { get; set; }
        public int? kills { get; set; }
        public int? last_hits { get; set; }
        public int? level { get; set; }
        public int? net_worth { get; set; }
        public int? obs_placed { get; set; }
        public long? party_id { get; set; }
        public int? party_size { get; set; }
        public List<Permanent_Buffs> permanent_buffs { get; set; }
        public int? pings { get; set; }
        public List<Purchase_Log> purchase_log { get; set; }
        public int? roshans_killed { get; set; }
        public List<Runes_Log> runes_log { get; set; }
        public int? tower_damage { get; set; }
        public int? towers_killed { get; set; }
        public int? xp_per_min { get; set; }
        public List<int> xp_t { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public bool radiant_win { get; set; }
        public int? total_gold { get; set; }
        public int? total_xp { get; set; }
        public int? neutral_kills { get; set; }
        public int? tower_kills { get; set; }
        public int? courier_kills { get; set; }
        public int? observer_kills { get; set; }
        public int? sentry_kills { get; set; }
        public int? roshan_kills { get; set; }
        public int? buyback_count { get; set; }
        public int? rank_tier { get; set; }
        public Benchmarks benchmarks { get; set; }
    }

    public class Benchmarks
    {
        public Gold_Per_Min gold_per_min { get; set; }
        public Xp_Per_Min xp_per_min { get; set; }
        public Kills_Per_Min kills_per_min { get; set; }
        public Last_Hits_Per_Min last_hits_per_min { get; set; }
        public Hero_Damage_Per_Min hero_damage_per_min { get; set; }
        public Hero_Healing_Per_Min hero_healing_per_min { get; set; }
        public Tower_Damage tower_damage { get; set; }
        public Stuns_Per_Min stuns_per_min { get; set; }
        public Lhten lhten { get; set; }
    }

    public class Gold_Per_Min
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Xp_Per_Min
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Kills_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Last_Hits_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Hero_Damage_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Hero_Healing_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Tower_Damage
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Stuns_Per_Min
    {
        public float raw { get; set; }
        public float pct { get; set; }
    }

    public class Lhten
    {
        public int raw { get; set; }
        public float pct { get; set; }
    }

    public class Additional_Units
    {
        public string unitname { get; set; }
        public int? item_0 { get; set; }
        public int? item_1 { get; set; }
        public int? item_2 { get; set; }
        public int? item_3 { get; set; }
        public int? item_4 { get; set; }
        public int? item_5 { get; set; }
        public int? backpack_0 { get; set; }
        public int? backpack_1 { get; set; }
        public int? backpack_2 { get; set; }
        public int? item_neutral { get; set; }
    }

    public class Permanent_Buffs
    {
        public int? permanent_buff { get; set; }
        public int? stack_count { get; set; }
    }

    public class Purchase_Log
    {
        public int? time { get; set; }
        public string key { get; set; }
        public int? charges { get; set; }
    }

    public class Runes_Log
    {
        public int? time { get; set; }
        public int? key { get; set; }
    }

}
