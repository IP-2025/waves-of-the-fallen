using Godot;

public partial class Archer : DefaultPlayer
{
    public Archer()
    {
        CharacterManager characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        MaxHealth = characterManager.LoadHealthByID("1");
        Speed = characterManager.LoadHealthByID("1");
    }

    public override void _Ready()
    {
        base._Ready();

        var healthNode = GetNode<Health>("Health");
        healthNode.max_health = MaxHealth;
        healthNode.ResetHealth();

        GD.Print($"Archer initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
    }

    public override void UseAbility()
    {
        //TODO: Implement Archer's ability
    }
}