using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class DotaPlayerProfileModel
    {
        public DotaPlayerProfileInfo? profile { get; set; } = new DotaPlayerProfileInfo();

        [JsonConverter(typeof(SafeIntConverter))]
        public int rank_tier { get; set; }

        [JsonConverter(typeof(SafeIntConverter))]
        public int leaderboard_rank { get; set; }
    }

    public class DotaPlayerProfileInfo
    {
        [JsonConverter(typeof(SafeStringConverter))]
        public string account_id { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string personaname { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeBoolConverter))]
        public bool plus { get; set; }

        [JsonConverter(typeof(SafeStringConverter))]
        public string steamid { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string avatar { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string avatarmedium { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string avatarfull { get; set; } = string.Empty;

        [JsonConverter(typeof(SafeStringConverter))]
        public string profileurl { get; set; } = string.Empty;
    }
}
