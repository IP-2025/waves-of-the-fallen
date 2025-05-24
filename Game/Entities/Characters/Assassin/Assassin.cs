using Godot;

public partial class Assassin : DefaultPlayer
{
    public Assassin()
    {
        CharacterManager characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        MaxHealth = characterManager.LoadHealthByID("2");
        Speed = characterManager.LoadHealthByID("2");
    }

    public override void _Ready()
    {
        base._Ready();

        var healthNode = GetNode<Health>("Health");
        healthNode.max_health = MaxHealth;
        healthNode.ResetHealth();

        GD.Print($"Assassin initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
    }

    public override void UseAbility()
    {
        //TODO: Implement Assassin's ability
    }
}