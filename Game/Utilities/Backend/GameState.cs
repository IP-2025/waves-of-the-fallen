using System.Collections.Generic;

namespace Game.Utilities.Backend;

public static class GameState
{
    public static ConnectionState CurrentState { get; set; } = ConnectionState.Online;
}