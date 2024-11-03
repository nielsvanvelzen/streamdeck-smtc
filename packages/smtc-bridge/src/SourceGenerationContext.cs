using System.Text.Json.Serialization;

namespace SmtcBridge;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(MediaPropertiesDto))]
internal partial class SourceGenerationContext : JsonSerializerContext;
