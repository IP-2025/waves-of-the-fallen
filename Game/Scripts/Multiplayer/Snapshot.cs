using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Snapshot {
    public ulong Tick;
    public List<EntitySnapshot> Entities = new List<EntitySnapshot>();
    public Snapshot(ulong tick) { Tick = tick; }
}