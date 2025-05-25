using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaHeroDataResponse
    {
        public DotaHeroDataResult? result { get; set; }
    }

    public class DotaHeroDataResult
    {
        public DotaHeroData? data { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int status { get; set; }
    }

    public class DotaHeroData
    {
        public DotaHeroDataModel[]? heroes { get; set; } = [];
    }

    public class DotaHeroDataModel
    {
        public int id { get; set; }

        public string name_loc { get; set; } = string.Empty;

        public string bio_loc { get; set; } = string.Empty;

        public FacetData[]? facets { get; set; } = [];

        public double str_base { get; set; }

        public double str_gain { get; set; }

        public double agi_base { get; set; }

        public double agi_gain { get; set; }

        public double int_base { get; set; }

        public double int_gain { get; set; }

        public double damage_min { get; set; }

        public double damage_max { get; set; }

        public double attack_rate { get; set; }

        public double attack_range { get; set; }

        public double projectile_speed { get; set; }

        public double armor { get; set; }

        public double magic_resistance { get; set; }

        public double movement_speed { get; set; }

        public double turn_rate { get; set; }

        public double sight_range_day { get; set; }

        public double sight_range_night { get; set; }

        public double max_health { get; set; }

        public double health_regen { get; set; }

        public double max_mana { get; set; }

        public double mana_regen { get; set; }

        public AbilityData[]? abilities { get; set; } = [];

        public AbilityData[]? talents { get; set; } = [];

        public FacetAbilityData[]? facet_abilities { get; set; } = [];
    }

    public class FacetData
    {
        public int color { get; set; }

        public string title_loc { get; set; } = string.Empty;

        public string description_loc { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;

        public string icon { get; set; } = string.Empty;

        public int gradient_id { get; set; }

        public int index { get; set; }
    }

    public class AbilityData
    {
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string name_loc { get; set; } = string.Empty;

        public string desc_loc { get; set; } = string.Empty;

        public string lore_loc { get; set; } = string.Empty;

        public string[]? notes_loc { get; set; } = [];

        public string shard_loc { get; set; } = string.Empty;

        public string scepter_loc { get; set; } = string.Empty;

        public string[]? facets_loc { get; set; } = [];

        public int type { get; set; }

        public string behavior { get; set; } = string.Empty;

        public int target_team { get; set; }

        public int target_type { get; set; }

        public int damage { get; set; }

        public int immunity { get; set; }

        public int dispellable { get; set; }

        public int max_level { get; set; }

        public double[]? cast_ranges { get; set; } = [];

        public double[]? cast_points { get; set; } = [];

        public double[]? channel_times { get; set; } = [];

        public double[]? cooldowns { get; set; } = [];

        public double[]? durations { get; set; } = [];

        public double[]? damages { get; set; } = [];

        public double[]? mana_costs { get; set; } = [];

        public double[]? gold_costs { get; set; } = [];

        public double[]? health_costs { get; set; } = [];

        public SpecialValueData[]? special_values { get; set; } = [];

        public bool ability_has_scepter { get; set; }

        public bool ability_has_shard { get; set; }

        public bool ability_is_granted_by_scepter { get; set; }

        public bool ability_is_granted_by_shard { get; set; }

        public bool ability_is_innate { get; set; }
    }

    public class SpecialValueData
    {
        public string name { get; set; } = string.Empty;

        public double[]? values_float { get; set; } = [];

        public bool is_percentage { get; set; }

        public string heading_loc { get; set; } = string.Empty;

        public BonusData[]? bonuses { get; set; } = [];

        public double[]? values_shard { get; set; } = [];

        public double[]? values_scepter { get; set; } = [];

        public FacetBonusData? facet_bonus { get; set; }

        public string required_facet { get; set; } = string.Empty;
    }

    public class BonusData
    {
        public string name { get; set; } = string.Empty;
        public double value { get; set; }
        public int operation { get; set; }
    }

    public class FacetBonusData
    {
        public string name { get; set; } = string.Empty;

        public double[]? values { get; set; } = [];

        public int operation { get; set; }
    }

    public class FacetAbilityData
    {
        public AbilityData[]? abilities { get; set; } = [];
    }
}
