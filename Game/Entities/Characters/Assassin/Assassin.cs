using Godot;

public partial class Assassin : DefaultPlayer
{
    public override void _Ready()
    {
        HttpRequest = GetNodeOrNull<HttpRequest>("HttpRequest");
        if (HttpRequest == null)
        {
            GD.PrintErr("HttpRequest node not found!");
            return;
        }

        base._Ready();

        MaxHealth = CharacterManager.LoadHealthByID("2");
        Speed = CharacterManager.LoadSpeedByID("2");

        var healthNode = GetNode<Health>("Health");
        healthNode.max_health = MaxHealth;
        healthNode.ResetHealth();

        GD.Print($"Assassin initialized. Speed: {Speed}, MaxHealth: {MaxHealth}");
    }

    protected override void UseAbility()
    {
        //TODO: Implement Assassin's ability
    }
}