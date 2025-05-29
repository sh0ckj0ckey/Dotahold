using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaPlayerWinLoseModel
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int win { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int lose { get; set; }
    }
}
