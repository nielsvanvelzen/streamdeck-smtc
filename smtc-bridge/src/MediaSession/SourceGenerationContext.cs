using System.Text.Json.Serialization;

namespace SmtcBridge.MediaSession;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(MediaPropertiesDto))]
internal partial class SourceGenerationContext : JsonSerializerContext;
