using System.Text.Json;

public static class Serializer
{
    public static byte[] Serialize<T>(T obj)
        => JsonSerializer.SerializeToUtf8Bytes(obj);

    public static T Deserialize<T>(byte[] data)
        => JsonSerializer.Deserialize<T>(data);
}
