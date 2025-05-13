using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaHeroRankingResponse
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int hero_id { get; set; }

        public DotaHeroRankingModel[]? rankings { get; set; }
    }

    public class DotaHeroRankingModel
    {
        [JsonConverter(typeof(SafeStringConverter))]
        public string account_id { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeIntConverter))]
        public int score { get; set; }

        [JsonConverter(typeof(SafeStringConverter))]
        public string personaname { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string avatar { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string rank_tier { get; set; } = string.Empty;
    }
}
