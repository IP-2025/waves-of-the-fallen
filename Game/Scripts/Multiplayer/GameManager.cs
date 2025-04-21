using System;
using System.Collections.Generic;
using Godot;

// DO THIS!!!
// go to Godot -> Project (top menu bar) -> Project settings -> Globals -> Autoload -> add GameManager.cs
// This file is for centralized, global player management. Because thats what game managers do
// This class manages the syncoronization of the game state between the server and clients

public partial class GameManager : Node
{
    // List to store all player Infos in this lobby
    public static List<PlayerInfo> Players = new List<PlayerInfo>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() { }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) { }
}




