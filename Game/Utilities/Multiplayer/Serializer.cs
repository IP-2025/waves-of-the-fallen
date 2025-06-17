using System;
using System.Text.Json;
using System.Text.Json.Serialization;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Metadata,
    IncludeFields = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase
)]
[JsonSerializable(typeof(Command))]
[JsonSerializable(typeof(Snapshot))]
internal partial class MyJsonContext : JsonSerializerContext
{
    // compiler fills this i think
}

public static class Serializer
{
    public static byte[] Serialize<T>(T obj) =>
        JsonSerializer.SerializeToUtf8Bytes(
            obj,
            typeof(T) == typeof(Command)
                ? MyJsonContext.Default.Command
                : MyJsonContext.Default.Snapshot
        );

    public static T Deserialize<T>(byte[] data) =>
        (T)JsonSerializer.Deserialize(
            data,
            typeof(T) == typeof(Command)
                ? MyJsonContext.Default.Command
                : MyJsonContext.Default.Snapshot
        );
} 
