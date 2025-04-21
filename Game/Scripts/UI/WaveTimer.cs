using System.Diagnostics;
using Godot;

public partial class WaveTimer : Node2D
{
	public int wave_counter = 0;
	public int second_counter = 0;

	public override void _Ready()
	{
		Timer wave_timer = GetNode<Timer>("WaveTimer");
		wave_timer.Timeout += OnTimerTimeout;
	}


	private void OnTimerTimeout()
	{
		second_counter++;
		if (second_counter >= 15)
		{
			foreach (Node2D enemy in GetTree().GetNodesInGroup("Enemies"))
			{
				Debug.Print("Deleted enemy");
				enemy.QueueFree();
			}
			second_counter = 0;
			wave_counter++;
		}
	}

}
