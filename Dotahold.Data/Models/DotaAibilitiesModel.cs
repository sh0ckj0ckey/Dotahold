using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaAibilitiesModel
    {
        public string[]? abilities { get; set; } = [];

        public DotaAibilitiesFacets[]? facets { get; set; } = [];
    }

    public class DotaAibilitiesFacets
    {
        [JsonConverter(typeof(SafeIntConverter))]
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string icon { get; set; } = string.Empty;

        public string color { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeIntConverter))]
        public int gradient_id { get; set; }

        public string title { get; set; } = string.Empty;

        public string description { get; set; } = string.Empty;
    }
}
