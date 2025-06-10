using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    [JsonSerializable(typeof(Dictionary<string, DotaHeroModel>))]
    [JsonSerializable(typeof(Dictionary<string, DotaItemModel>))]
    [JsonSerializable(typeof(Dictionary<string, DotaAibilitiesModel>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(Dictionary<string, long>))]
    [JsonSerializable(typeof(DotaHeroDataResponse))]
    [JsonSerializable(typeof(DotaHeroRankingResponse))]
    [JsonSerializable(typeof(SteamNumberOfCurrentPlayersResponse))]
    [JsonSerializable(typeof(DotaPlayerWinLoseModel))]
    [JsonSerializable(typeof(DotaPlayerOverallPerformanceModel[]))]
    [JsonSerializable(typeof(DotaPlayerHeroPerformanceModel[]))]
    [JsonSerializable(typeof(DotaPlayerProfileModel))]
    [JsonSerializable(typeof(DotaMatchModel[]))]
    internal partial class SourceGenerationContext : JsonSerializerContext { }
}
