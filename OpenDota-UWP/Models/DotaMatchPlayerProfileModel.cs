using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    public class DotaMatchPlayerProfileModel
    {
        public object solo_competitive_rank { get; set; }

        /// <summary>
        /// 冠绝一世排名
        /// </summary>
        public int? leaderboard_rank { get; set; }

        public Mmr_Estimate mmr_estimate { get; set; }
        public object competitive_rank { get; set; }

        /// <summary>
        /// 分段，十位数表示徽章，个位数表示星级
        /// </summary>
        public int? rank_tier { get; set; }
        public Profile profile { get; set; }
    }

    public class Mmr_Estimate
    {
        public int estimate { get; set; }
    }

    public class Profile
    {
        public long account_id { get; set; }
        public string personaname { get; set; }
        public object name { get; set; }
        public bool? plus { get; set; }
        public object cheese { get; set; }
        public string steamid { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string profileurl { get; set; }
        public object last_login { get; set; }
        public object loccountrycode { get; set; }
        public object status { get; set; }
        public object is_contributor { get; set; }
        public object is_subscriber { get; set; }
    }

}
