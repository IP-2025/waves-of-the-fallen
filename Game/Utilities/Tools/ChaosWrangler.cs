using Godot;
using System;
using Game.Utilities.Multiplayer;

public partial class ChaosWrangler : Node
{
    public static ChaosWrangler Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void StartGame()
    {
        var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();
        NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
        NetworkManager.Instance.Rpc("NotifyGameStart");
        // SoundManager.Instance.PlayUI();
    }
}
