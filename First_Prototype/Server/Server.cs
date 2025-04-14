#nullable enable // Enable nullable reference types
using System;
using System.Collections.Generic;
using System.IO;                   
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;              

Dictionary<int, int[]> positions = new()
{
    { 0, new int[] { 100, 100 } },
    { 1, new int[] { 300, 300 } }
};

TcpListener server = new(IPAddress.Any, 5555);
server.Start();
Console.WriteLine("Server läuft...");

int playerId = 0;

while (playerId < 2)
{
    var client = server.AcceptTcpClient();
    Console.WriteLine("Client verbunden");

    int currentId = playerId;
    new Thread(() =>
    {
        var stream = client.GetStream();
        var writer = new StreamWriter(stream) { AutoFlush = true };
        var reader = new StreamReader(stream);

        // Send the starting position of the current player
        writer.WriteLine(JsonSerializer.Serialize(positions[currentId]));

        while (true)
        {
            try
            {
                string? data = reader.ReadLine();
                if (data == null) break;
                positions[currentId] = JsonSerializer.Deserialize<int[]>(data)!;

                int[] otherPos = positions[1 - currentId];
                writer.WriteLine(JsonSerializer.Serialize(otherPos)); // Send the position of the other player
            }
            catch { break; }
        }

        client.Close();
    }).Start();

    playerId++;
}
