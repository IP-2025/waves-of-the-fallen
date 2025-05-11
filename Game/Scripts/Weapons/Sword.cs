using Godot;
using System;
public partial class Sword : MeleeWeapon
{
	private AnimationPlayer SwordAnimationPlayer;
	[Export] public int Damage = 20;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SwordAnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private async void OnTimerTimeoutAttack()
	{
		SwordAnimationPlayer.Play("SwordAttack"); 
		await ToSignal(GetTree().CreateTimer(0.2), "timeout");
		MeleeAttack(Damage);
	}
	
}
