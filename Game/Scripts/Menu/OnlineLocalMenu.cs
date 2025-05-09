using Godot;
using System;

public partial class OnlineLocalMenu : Control
{
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
    NetworkManager.Instance.StartHeadlessServer(true);

    if (NetworkManager.Instance.IsPortOpen(NetworkManager.Instance.GetServerIPAddress(), NetworkManager.Instance.RPC_PORT, 500))
    {
      NetworkManager.Instance.InitClient(NetworkManager.Instance.GetServerIPAddress());
    }
    else
    {
      var timer = new Timer();
      AddChild(timer);
      timer.WaitTime = 0.5f;
      timer.OneShot = true;
      timer.Timeout += () => NetworkManager.Instance.InitClient(NetworkManager.Instance.GetServerIPAddress());
      timer.Start();
    }

    var timer2 = new Timer();
      AddChild(timer2);
      timer2.WaitTime = 1;
      timer2.OneShot = true;
      timer2.Timeout += () => NetworkManager.Instance.Rpc("NotifyGameStart");
      timer2.Start();
    
  }
}
