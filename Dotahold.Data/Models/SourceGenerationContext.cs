﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    [JsonSerializable(typeof(Dictionary<string, DotaHeroModel>))]
    [JsonSerializable(typeof(Dictionary<string, DotaItemModel>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(Dictionary<string, long>))]
    [JsonSerializable(typeof(DotaHeroDataResponse))]
    [JsonSerializable(typeof(DotaHeroRankingResponse))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
