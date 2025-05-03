using System;
using System.Text.Json;


public static class Serializer
{
    private static readonly JsonSerializerOptions _opts = new JsonSerializerOptions
    {
        IncludeFields = true,
        PropertyNameCaseInsensitive = true
    };

    public static byte[] Serialize<T>(T obj)
        => JsonSerializer.SerializeToUtf8Bytes(obj, _opts);

    public static T Deserialize<T>(byte[] data)
        => JsonSerializer.Deserialize<T>(data, _opts)
           ?? throw new InvalidOperationException("Deserializing returned null");
}

