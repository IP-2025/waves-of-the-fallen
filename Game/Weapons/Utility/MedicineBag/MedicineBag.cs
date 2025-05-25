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
		if (Multiplayer.IsServer())
			Use();
	}

	protected void Use()
	{
		PathFollow2D path = GetNode<PathFollow2D>("Path2D/PathFollow2D");
		path.ProgressRatio = GD.Randf();
		Area2D utilInstance = medicine.Instantiate() as Area2D;

		utilInstance.GlobalPosition = path.GlobalPosition;

		ulong id = utilInstance.GetInstanceId();
		utilInstance.Name = $"Medicine_{id}";
		Server.Instance.Entities[(long)id] = utilInstance;

		GetTree().CurrentScene.AddChild(utilInstance);
	}
}
