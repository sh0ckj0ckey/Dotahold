using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaMatchHeroPlayedModel
    {
        public string hero_id { get; set; }
        public double? last_played { get; set; }
        public double? games { get; set; }
        public double? win { get; set; }
        public double? with_games { get; set; }
        public double? with_win { get; set; }
        public double? against_games { get; set; }
        public double? against_win { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroCoverImage { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroName { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public string sWinRate { get; set; } = string.Empty;
    }

}
