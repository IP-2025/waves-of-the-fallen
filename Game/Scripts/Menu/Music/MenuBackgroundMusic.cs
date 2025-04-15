using Godot;
using System;

public partial class MenuBackgroundMusic : AudioStreamPlayer
{
 private int menuBackgroundMusicBusIndex;

 public override void _Ready()
{
	menuBackgroundMusicBusIndex=AudioServer.GetBusIndex("MenuBackgroundMusicBus");
	// Volume is temporarily set low so its not to annoying
	AudioServer.SetBusVolumeDb(menuBackgroundMusicBusIndex,Mathf.LinearToDb(0.1f));
}

}
