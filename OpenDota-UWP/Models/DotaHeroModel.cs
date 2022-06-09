using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaHeroModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string localized_name { get; set; }
        public string primary_attr { get; set; }
        public string attack_type { get; set; }
        public string[] roles { get; set; }
        public string img { get; set; }
        public string icon { get; set; }
        public double base_health { get; set; }
        public double base_health_regen { get; set; }
        public double base_mana { get; set; }
        public double base_mana_regen { get; set; }
        public double base_armor { get; set; }
        public double base_mr { get; set; }
        public double base_attack_min { get; set; }
        public double base_attack_max { get; set; }
        public double base_str { get; set; }
        public double base_agi { get; set; }
        public double base_int { get; set; }
        public double str_gain { get; set; }
        public double agi_gain { get; set; }
        public double int_gain { get; set; }
        public double attack_range { get; set; }
        public double projectile_speed { get; set; }
        public double attack_rate { get; set; }
        public double move_speed { get; set; }
        public double turn_rate { get; set; }
        public bool? cm_enabled { get; set; }
        public double legs { get; set; }
    }

}
