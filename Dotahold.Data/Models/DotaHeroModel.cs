using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaHeroModel
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int id { get; set; }

        public string? name { get; set; } = string.Empty;
        public string? localized_name { get; set; } = string.Empty;
        public string? primary_attr { get; set; } = string.Empty;
        public string? attack_type { get; set; } = string.Empty;
        public string[]? roles { get; set; } = [];
        public string? img { get; set; } = string.Empty;
        public string? icon { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_health { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_health_regen { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_mana { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_mana_regen { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_armor { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_mr { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_attack_min { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_attack_max { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_str { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_agi { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_int { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double str_gain { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double agi_gain { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double int_gain { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double attack_range { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double projectile_speed { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double attack_rate { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double base_attack_time { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double attack_point { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double move_speed { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double turn_rate { get; set; }

        [JsonConverter(typeof(SafeBoolConverter))]
        public bool cm_enabled { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double legs { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double day_vision { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double night_vision { get; set; }
    }
}
