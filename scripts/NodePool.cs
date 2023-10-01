using System.Collections.Generic;
using Godot;

public class NodePool<T> where T : Node3D {
    private readonly string scene;
    private readonly Stack<T> pool = new();
    private readonly HashSet<T> spawned = new();
    public NodePool(string scene) {
        this.scene = scene;
    }

    public T Spawn(Node parent) {
        T res = null;
        if (pool.TryPop(out res)) {
            res.ProcessMode = Node.ProcessModeEnum.Inherit;
            res.Visible = true;
            if (!parent.IsAncestorOf(res)) {
                // FIXME: would this reparent or create a copy? not sure yet.
                parent.AddChild(res);
            }
        } else {
            res = Utils.Instantiate<T>(scene);
            parent.AddChild(res);
        }
        spawned.Add(res);
        if (res is IPoolable pres) {
            pres.OnSpawn();
        }
        return res;
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