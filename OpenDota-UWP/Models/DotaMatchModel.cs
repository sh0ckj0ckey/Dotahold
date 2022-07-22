using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDota_UWP.Models
{
    //public class DotaRecentMatchesModel
    //{
    //    public List<DotaRecentMatchModel> vRecentMatches { get; set; }
    //}

    public class DotaRecentMatchModel
    {
        public long? match_id { get; set; }
        public int? player_slot { get; set; }
        public bool? radiant_win { get; set; }
        public int? duration { get; set; }
        public int? game_mode { get; set; }

        /// <summary>
        /// 比赛类型，比如练习赛、锦标赛、天梯等
        /// </summary>
        public int? lobby_type { get; set; }

        public int? hero_id { get; set; }

        /// <summary>
        /// 开始时间，自1970年开始计秒
        /// </summary>
        public long? start_time { get; set; }

        public int? version { get; set; }
        public int? kills { get; set; }
        public int? deaths { get; set; }
        public int? assists { get; set; }

        /// <summary>
        /// 判定N/H/VH局
        /// </summary>
        public object skill { get; set; }

        public object average_rank { get; set; }
        public int? xp_per_min { get; set; }
        public int? gold_per_min { get; set; }
        public long? hero_damage { get; set; }
        public long? tower_damage { get; set; }
        public long? hero_healing { get; set; }
        public int? last_hits { get; set; }
        public int? lane { get; set; }
        public int? lane_role { get; set; }
        public bool? is_roaming { get; set; }
        public string cluster { get; set; }
        public string leaver_status { get; set; }
        public int? party_size { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroCoverImage { get; set; } = string.Empty;

        [Newtonsoft.Json.JsonIgnore]
        public string sHeroName { get; set; } = string.Empty;


    }

}
