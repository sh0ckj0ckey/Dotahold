using System;
using System.Collections.Generic;
using Dotahold.Data.Models;

namespace Dotahold.Models
{
    public class PlayerOverallPerformanceModel(DotaPlayerOverallPerformanceModel overall)
    {
        public static bool IsFieldAvailable(string fieldName) => _fieldNames.ContainsKey(fieldName);

        private static readonly Dictionary<string, string> _fieldNames = new()
        {
            {"kills", "Kills"},
            {"deaths", "Deaths"},
            {"assists", "Assists"},
            {"gold_per_min", "GPM"},
            {"xp_per_min", "XPM"},
            {"last_hits", "Last Hits"},
            {"denies", "Denies"},
            {"level", "Level"},
            {"hero_damage", "Hero Damage"},
            {"tower_damage", "Tower Damage"},
            {"hero_healing", "Hero Healing"},
            {"KDA", "KDA"},
        };

        public string FieldName { get; private set; } = _fieldNames.TryGetValue(overall.field, out string? fieldName) ? fieldName : overall.field.ToUpper().Replace("_", " ");

        public string FieldIcon { get; private set; } = overall.field switch
        {
            "kills" => "\uE8F0",
            "deaths" => "\uE624",
            "assists" => "\uE8E1",
            "gold_per_min" => "\uEAFC",
            "xp_per_min" => "\uEAFC",
            "last_hits" => "\uF138",
            "denies" => "\uE8C9",
            "level" => "\uE752",
            "hero_damage" => "\uEA92",
            "tower_damage" => "\uECAD",
            "hero_healing" => "\uE95E",
            "KDA" => "\uF272",
            _ => "",
        };

        public double Average { get; private set; } = overall.n > 0 ? Math.Floor((overall.sum / overall.n) * 10) / 10 : Math.Floor(overall.sum * 10) / 10;
    }
}
