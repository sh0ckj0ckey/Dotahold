using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaPlayerOverallPerformanceModel
    {
        public string field { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double n { get; set; }

        [JsonConverter(typeof(SafeDoubleConverter))]
        public double sum { get; set; }
    }
}
