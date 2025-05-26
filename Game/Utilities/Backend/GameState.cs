using System.Collections.Generic;

namespace Game.Utilities.Backend;

public static class GameState
{
    public static ConnectionState CurrentState { get; set; } = ConnectionState.Online;

    // Score for each player Spieler (Key: PlayerId, Value: Score)
    public static Dictionary<long, int> PlayerScores { get; } = new();
}