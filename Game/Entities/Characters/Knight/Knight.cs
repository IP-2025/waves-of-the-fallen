using Godot;

public partial class Knight : DefaultPlayer
{
    public Knight()
    {
        CharacterManager characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        MaxHealth = characterManager.LoadHealthByID("3");
        Speed = characterManager.LoadHealthByID("3");
    }

    public override void _Ready()
    {
        base._Ready();

        var healthNode = GetNode<Health>("Health");
        healthNode.max_health = MaxHealth;
        healthNode.ResetHealth();

        GD.Print($"Knight initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
    }

    public override void UseAbility()
    {
        //TODO: Implement Knight's ability
    }
}