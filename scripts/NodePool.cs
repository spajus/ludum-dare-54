using System.Collections.Generic;
using Godot;

public class NodePool<T> where T : Node3D {
    private readonly string scene;
    private readonly Stack<T> pool = new();
    private readonly HashSet<T> spawned = new();
    public NodePool(string scene) {
        this.scene = scene;
    }

    public T Spawn() {
        if (pool.TryPop(out var res)) {
            res.ProcessMode = Node.ProcessModeEnum.Inherit;
            res.Visible = true;
            spawned.Add(res);
            return res;
        }
        var newInst = Utils.Instantiate<T>(scene);
        spawned.Add(newInst);
        return newInst;
    }

    public void Despawn(T item) {
        pool.Push(item);
        if (!spawned.Remove(item)) {
            GD.PrintErr("Trying to despawn an item that is not pooled! ", item);
        }
        item.Visible = false;
        item.ProcessMode = Node.ProcessModeEnum.Disabled;
    }

    public void DespawnAll() {
        var toDespawn = new List<T>(spawned);
        foreach (var item in toDespawn) {
            Despawn(item);
        }
    }
}