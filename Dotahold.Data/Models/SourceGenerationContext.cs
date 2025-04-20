using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Dotahold.Data.Models
{
    [JsonSerializable(typeof(Dictionary<string, DotaHeroModel>))]
    [JsonSerializable(typeof(Dictionary<string, DotaItemModel>))]
    [JsonSerializable(typeof(Dictionary<string, string>))]
    [JsonSerializable(typeof(Dictionary<string, long>))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }
}
