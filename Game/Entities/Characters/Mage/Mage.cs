using Godot;

public partial class Mage : DefaultPlayer
{
    public Mage()
    {
        CharacterManager characterManager = GetNode<CharacterManager>("/root/CharacterManager");
        MaxHealth = characterManager.LoadHealthByID("4");
        Speed = characterManager.LoadHealthByID("4");
    }

    public override void _Ready()
    {
        base._Ready();

        var healthNode = GetNode<Health>("Health");
        healthNode.max_health = MaxHealth;
        healthNode.ResetHealth();

        GD.Print($"Mage initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
    }

    public override void UseAbility()
    {
        //TODO: Implement Mage's ability
    }
}