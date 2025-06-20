namespace Game.Utilities.Backend;

public static class GameState
{
    public static ConnectionState CurrentState { get; set; } = ConnectionState.Online;
    public static string LobbyCode { get; set; } = "";
}