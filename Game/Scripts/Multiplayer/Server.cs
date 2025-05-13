using Godot;
using System.Collections.Generic;
using System.Reflection.Metadata;


public partial class Server : Node
{
    public static Server Instance;

    private bool enableDebug = false;
    public Dictionary<long, int> PlayerSelections = new Dictionary<long, int>();
    public Dictionary<long, Node2D> Entities = new Dictionary<long, Node2D>();
    private static readonly Dictionary<string, EntityType> ScenePathToEntityType = new()
    {
        { "res://Scenes/Characters/default_player.tscn", EntityType.DefaultPlayer },
        { "res://Scenes/Characters/archer.tscn", EntityType.Archer },
        { "res://Scenes/Characters/knight.tscn", EntityType.Knight },
        { "res://Scenes/Characters/assassin.tscn", EntityType.Assassin }, // Hinzugefügt
        { "res://Scenes/Characters/mage.tscn", EntityType.Mage },         // Hinzugefügt
        { "res://Scenes/Characters/default_enemy.tscn", EntityType.DefaultEnemy },
        { "res://Scenes/Characters/mounted_enemy.tscn", EntityType.MountedEnemy },
        { "res://Scenes/Characters/ranged_enemy.tscn", EntityType.RangedEnemy },
        { "res://Scenes/Characters/rider_enemy.tscn", EntityType.RiderEnemy },
        { "res://Scenes/Weapons/bow.tscn", EntityType.Bow },
        { "res://Scenes/Weapons/bow_arrow.tscn", EntityType.BowArrow },
        { "res://Scenes/Weapons/crossbow.tscn", EntityType.Crossbow },
        { "res://Scenes/Weapons/crossbow_arrow.tscn", EntityType.CrossbowArrow },
        { "res://Scenes/Weapons/kunai.tscn", EntityType.Kunai },
        { "res://Scenes/Weapons/kunai_projectile.tscn", EntityType.KunaiProjectile },
        { "res://Scenes/Weapons/firestaff.tscn", EntityType.FireStaff},
        { "res://Scenes/Weapons/fireball.tscn", EntityType.FireBall}
    };
    public override void _Ready()
    {
        Instance = this;
    }

    public void ProcessCommand(Command cmd)
    {
        if (!Entities.TryGetValue(cmd.EntityId, out var entity))
        {
            DebugIt($"Entity {cmd.EntityId} not found in Entities dictionary");
            return;
        }

        if (cmd.Type == CommandType.Move && cmd.MoveDir.HasValue)
        {
            // get move vector from command
            var dir = cmd.MoveDir.Value;
            if (dir.Length() > 1f)
            {
                dir = dir.Normalized();
            }

            // try to get the joystick node from the active player
            var joystick = entity.GetNodeOrNull<Joystick>("Joystick");
            if (joystick != null)
            {
                joystick.PosVector = dir;
                
                DebugIt($"Set Joystick.PosVector = {dir} on EntityID {cmd.EntityId}");
            }
        }
        else if (cmd.Type == CommandType.Shoot)
        {
            // Tmaybe we need, maybe we don't
        }
    }

    public byte[] GetSnapshot(ulong tick)
    {


        // serialize and build snapshot
        var snap = new Snapshot(tick);
        var toRemove = new List<long>();
        foreach (var kv in Entities)
        {
            var node = kv.Value;
            // check for correct scene paths
            // GD.Print("  scenePath = " + node.SceneFilePath);

            if (node == null || !IsInstanceValid(node))
            {
                toRemove.Add(kv.Key);
                continue;
            }

            string scenePath = node.SceneFilePath;
            var id = kv.Key;

            if (!ScenePathToEntityType.TryGetValue(scenePath, out var entityType))
            {
                GD.PrintErr($"Unknown ScenePath: {scenePath}");
                continue;
            }

            // Find WaveTimer as a child of the current camera
            int waveCount = 0;
            int secondsLeft = 0;
            bool graceTime = false;
            var cam = GetViewport().GetCamera2D();
            if (cam != null)
            {
                var waveTimer = cam.GetNodeOrNull<WaveTimer>("WaveTimer");
                if (waveTimer != null)
                {
                    waveCount = waveTimer.waveCounter;
                    secondsLeft = waveTimer.secondCounter;
                    graceTime = waveTimer.isPaused;
                }
            }

            // Get health
            var healthNode = node.GetNodeOrNull<Health>("Health");
            float health = healthNode != null ? healthNode.health : 0f;

            // get players weapons
            long? owner = null;
            int? slotIx = null;
            if (node.HasMeta("OwnerId"))
            {
                owner = (long)node.GetMeta("OwnerId");
                slotIx = (int)node.GetMeta("SlotIndex");
            }
            DebugIt($"Snapshot: Entity Name: {node.Name}, Position: {node.Position}, ID: {id}");
            snap.Entities.Add(new EntitySnapshot(
                id,
                node.Position,
                node.Rotation,
                node.Scale,
                health,
                entityType,
                waveCount,
                secondsLeft,
                graceTime,
                owner, // nullable
                slotIx // nullable
            ));
        }

        // remove invalid entities, for example killed enemies
        foreach (var id in toRemove)
        {
            Entities.Remove(id);
        }

        return Serializer.Serialize(snap);
    }

    private void DebugIt(string message)
    {
        if (enableDebug)
            GD.Print($"Server: {message}");
    }
}