using Godot;
using System;
using System.Threading.Tasks;

public partial class OnlineLocalMenu : Control
{
  bool _headlessIsReady = false;

  public override void _Ready()
  {
    NetworkManager.Instance.HeadlessServerInitialized += OnHeadlessServerInitialized;
  }

  private void HeadlessServerInitializedEventHandler()
  {
    throw new NotImplementedException();
  }

  private void _on_button_back_onlineLocal_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/mainMenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_local_pressed()
  {
    var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Menu/localMenu.tscn");
    GetTree().ChangeSceneToPacked(scene);
  }
  private void _on_button_online_pressed()
  {

  }

  private void _on_button_solo_pressed()
  {
    Button soloButton = GetNode<Button>("MarginContainer2/VBoxContainer/MarginContainer/HBoxContainer2/Button_Solo");
    soloButton.Disabled = true;
    NetworkManager.Instance.StartHeadlessServer(true);
  }

  private void OnHeadlessServerInitialized()
  {
    var timer = new Timer();
    AddChild(timer);
    timer.WaitTime = 0.5f;
    timer.OneShot = true;

    timer.Timeout += () =>
    {
      NetworkManager.Instance.InitClient(NetworkManager.Instance.GetServerIPAddress());

      var characterManager = GetNode<CharacterManager>("/root/CharacterManager");
      int selectedCharacterId = characterManager.LoadLastSelectedCharacterID();

      Timer gameStartTimer = new Timer();
      AddChild(gameStartTimer);
      gameStartTimer.WaitTime = 0.5f;
      gameStartTimer.OneShot = true;
      gameStartTimer.Timeout += () => {
        NetworkManager.Instance.RpcId(1, "SelectCharacter", selectedCharacterId);
        NetworkManager.Instance.Rpc("NotifyGameStart");
      };
      gameStartTimer.Start();
    };

    timer.Start();
  }

  public override void _ExitTree()
  {
    NetworkManager.Instance.HeadlessServerInitialized -= OnHeadlessServerInitialized;
  }
}
