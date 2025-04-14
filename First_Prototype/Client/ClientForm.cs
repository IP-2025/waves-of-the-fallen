#nullable enable
using System;                     // For basic functions
using System.IO;                  // For StreamReader and StreamWriter
using System.Net.Sockets;        // For TcpClient
using System.Text.Json;          // For JsonSerializer
using System.Windows.Forms;      // For Form and Controls
using System.Drawing;            // For Brushes

public class ClientForm : Form
{
    TcpClient client = new();
    StreamReader reader;
    StreamWriter writer;
    int[] myPos;
    int[] otherPos = new int[] { 0, 0 };
    int playerId;

    public ClientForm()
    {
        this.Width = 640;
        this.Height = 480;
        this.Text = "Multiplayer Rectangle";

        client.Connect("127.0.0.1", 5555);
        var stream = client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream) { AutoFlush = true };

        string startData = reader.ReadLine();
        myPos = JsonSerializer.Deserialize<int[]>(startData)!;
        playerId = myPos[0] < 200 ? 0 : 1; // Set player ID based on starting position

        var timer = new Timer();
        timer.Interval = 33; // ~30 FPS
        timer.Tick += Update;
        timer.Start();

        this.KeyDown += MovePlayer; // Key input event
        this.DoubleBuffered = true; // Prevents flickering
    }

    private void MovePlayer(object? sender, KeyEventArgs e)
    {
        int speed = 5;
        if (playerId == 0) // Player 1 controls with WASD
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A) myPos[0] -= speed;
            if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D) myPos[0] += speed;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W) myPos[1] -= speed;
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S) myPos[1] += speed;
        }
        else // Player 2 controls with arrow keys
        {
            if (e.KeyCode == Keys.Left) myPos[0] -= speed;
            if (e.KeyCode == Keys.Right) myPos[0] += speed;
            if (e.KeyCode == Keys.Up) myPos[1] -= speed;
            if (e.KeyCode == Keys.Down) myPos[1] += speed;
        }
    }

    private void Update(object? sender, EventArgs e)
    {
        writer.WriteLine(JsonSerializer.Serialize(myPos));
        string? data = reader.ReadLine();
        if (data != null)
            otherPos = JsonSerializer.Deserialize<int[]>(data)!;
        this.Invalidate(); // Refresh the display
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        var myColor = playerId == 0 ? Brushes.Red : Brushes.Blue;
        var otherColor = playerId == 0 ? Brushes.Blue : Brushes.Red;

        g.FillRectangle(myColor, myPos[0], myPos[1], 50, 50); // Draw my rectangle
        g.FillRectangle(otherColor, otherPos[0], otherPos[1], 50, 50); // Draw the other rectangle
    }
}
