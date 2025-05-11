using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
    }

    public class FacetData
    {
        public int color { get; set; }

        public string title_loc { get; set; } = string.Empty;

        public string description_loc { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;

        /// <summary>
        /// {ConstantsCourier.ImageSourceDomain}/apps/dota2/images/dota_react/icons/facets/{icon}.png
        /// </summary>
        public string icon { get; set; } = string.Empty;

        public int gradient_id { get; set; }

        public int index { get; set; }
    }
}
