using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaHeroRankingModel
    {
        public string hero_id { get; set; }
        public List<RankingPlayer> rankings { get; set; }
    }

    public class RankingPlayer
    {
        public string account_id { get; set; }
        public string score { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string last_login { get; set; }
        public string rank_tier { get; set; }

        public int iRank { get; set; }
    }

}
