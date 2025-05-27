using Godot;
using Game.Utilities.Multiplayer;
public partial class MedicineBag : Area2D
{
	private PackedScene medicine = GD.Load<PackedScene>("res://Weapons/Utility/MedicineBag/medicine.tscn");

	private AnimatedSprite2D medicineBagSprite;

	public override void _Ready()
	{
		medicineBagSprite = GetNode<AnimatedSprite2D>("MedicineBagSprite");
	}


	public async void OnTimerTimeoutMedicineBag()
	{
		medicineBagSprite.Play();
		await ToSignal(GetTree().CreateTimer(1.2), "timeout");
		if (Multiplayer.IsServer() && GetNode<Health>("../../../Health").CurHealth > 0) // gets the healthnode of weaponowner, so medicine doesn't get thrown by server when dead
			Use();
	}

	protected void Use()
	{
		Area2D utilInstance = medicine.Instantiate() as Area2D;

		utilInstance.GlobalPosition = GlobalPosition;

		ulong id = utilInstance.GetInstanceId();
		utilInstance.Name = $"Medicine_{id}";
		Server.Instance.Entities[(long)id] = utilInstance;

		GetTree().CurrentScene.AddChild(utilInstance);
	}
}
