using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dotahold.Models
{
    [JsonSerializable(typeof(List<PlayerConnectRecord>))]
    internal partial class SourceGenerationContext : JsonSerializerContext { }
}
