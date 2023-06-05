using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dotahold.Models
{
    /// <summary>
    /// 用户的胜局败局数量
    /// </summary>
    public class DotaMatchWinLoseModel
    {
        /// <summary>
        /// 胜利
        /// </summary>
        public double win { get; set; }

        /// <summary>
        /// 失败
        /// </summary>
        public double lose { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string winRate { get; set; } = string.Empty;
    }
}
