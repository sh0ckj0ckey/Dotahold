using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaHeroInfoModel
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public Data data { get; set; }
        public int status { get; set; }
    }

    public class Data
    {
        public Hero[] heroes { get; set; }
    }

    public class Hero
    {
        public int id { get; set; }
        public string name { get; set; }
        public int order_id { get; set; }
        public string name_loc { get; set; }
        public string bio_loc { get; set; }
        public string hype_loc { get; set; }
        public string npe_desc_loc { get; set; }
        public int str_base { get; set; }
        public float str_gain { get; set; }
        public int agi_base { get; set; }
        public int agi_gain { get; set; }
        public int int_base { get; set; }
        public float int_gain { get; set; }
        public int primary_attr { get; set; }
        public int complexity { get; set; }
        public int attack_capability { get; set; }
        public int[] role_levels { get; set; }
        public int damage_min { get; set; }
        public int damage_max { get; set; }
        public float attack_rate { get; set; }
        public int attack_range { get; set; }
        public int projectile_speed { get; set; }
        public float armor { get; set; }
        public int magic_resistance { get; set; }
        public int movement_speed { get; set; }
        public float turn_rate { get; set; }
        public int sight_range_day { get; set; }
        public int sight_range_night { get; set; }
        public int max_health { get; set; }
        public int health_regen { get; set; }
        public int max_mana { get; set; }
        public float mana_regen { get; set; }
        public Ability[] abilities { get; set; }
        public Talent[] talents { get; set; }
    }

    public class Ability
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_loc { get; set; }
        public string desc_loc { get; set; }
        public string lore_loc { get; set; }
        public string[] notes_loc { get; set; }
        public string shard_loc { get; set; }
        public string scepter_loc { get; set; }
        public int type { get; set; }
        public string behavior { get; set; }
        public int target_team { get; set; }
        public int target_type { get; set; }
        public int flags { get; set; }
        public int damage { get; set; }
        public int immunity { get; set; }
        public int dispellable { get; set; }
        public int max_level { get; set; }
        public int[] cast_ranges { get; set; }
        public float[] cast_points { get; set; }
        public int[] channel_times { get; set; }
        public float[] cooldowns { get; set; }
        public int[] durations { get; set; }
        public int[] damages { get; set; }
        public int[] mana_costs { get; set; }
        public object[] gold_costs { get; set; }
        public Special_Values[] special_values { get; set; }
        public bool is_item { get; set; }
        public bool ability_has_scepter { get; set; }
        public bool ability_has_shard { get; set; }
        public bool ability_is_granted_by_scepter { get; set; }
        public bool ability_is_granted_by_shard { get; set; }
        public int item_cost { get; set; }
        public int item_initial_charges { get; set; }
        public long item_neutral_tier { get; set; }
        public int item_stock_max { get; set; }
        public int item_stock_time { get; set; }
        public int item_quality { get; set; }
    }

    public class Talent
    {
        public int id { get; set; }
        public string name { get; set; }
        public string name_loc { get; set; }
        public string desc_loc { get; set; }
        public string lore_loc { get; set; }
        public object[] notes_loc { get; set; }
        public string shard_loc { get; set; }
        public string scepter_loc { get; set; }
        public int type { get; set; }
        public string behavior { get; set; }
        public int target_team { get; set; }
        public int target_type { get; set; }
        public int flags { get; set; }
        public int damage { get; set; }
        public int immunity { get; set; }
        public int dispellable { get; set; }
        public int max_level { get; set; }
        public int[] cast_ranges { get; set; }
        public int[] cast_points { get; set; }
        public int[] channel_times { get; set; }
        public int[] cooldowns { get; set; }
        public int[] durations { get; set; }
        public int[] damages { get; set; }
        public int[] mana_costs { get; set; }
        public object[] gold_costs { get; set; }
        public Special_Values[] special_values { get; set; }
        public bool is_item { get; set; }
        public bool ability_has_scepter { get; set; }
        public bool ability_has_shard { get; set; }
        public bool ability_is_granted_by_scepter { get; set; }
        public bool ability_is_granted_by_shard { get; set; }
        public int item_cost { get; set; }
        public int item_initial_charges { get; set; }
        public long item_neutral_tier { get; set; }
        public int item_stock_max { get; set; }
        public int item_stock_time { get; set; }
        public int item_quality { get; set; }
    }

    public class Special_Values
    {
        public string name { get; set; }
        public float[] values_float { get; set; }
        public bool is_percentage { get; set; }
        public string heading_loc { get; set; }
        public object[] bonuses { get; set; }
    }
}
