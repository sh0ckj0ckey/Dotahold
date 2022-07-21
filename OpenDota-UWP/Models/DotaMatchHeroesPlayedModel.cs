using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaMatchHeroesPlayedModel
    {
        public List<HeroPlayed> vHeroesPlayed { get; set; }
    }

    public class HeroPlayed
    {
        public string hero_id { get; set; }
        public int last_played { get; set; }
        public int games { get; set; }
        public int win { get; set; }
        public int with_games { get; set; }
        public int with_win { get; set; }
        public int against_games { get; set; }
        public int against_win { get; set; }
    }

}
